using Azure.Core;
using CsvHelper;
using CsvHelper.Configuration;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
using FitnessFox.Data.Vitals;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.Services
{
    public class GoogleOptions
    {
        public string FileName { get; set; } = "fitnessfox-467923-9857f7a3ab7e.json";
    }
    public class GoogleSyncService : IGoogleSyncService
    {
        private readonly IFileService fileService;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IOptions<GoogleOptions> options;
        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        string ApplicationName = "FitnessFox";
        string SpreadsheetId = "1nrJfqZ-0mWHS9ChBthiyvPomOafk-46wepEVUm6g8hw";

        public GoogleSyncService(
            IFileService fileService,
            ApplicationDbContext applicationDbContext,
            IOptions<GoogleOptions> options)
        {
            this.fileService = fileService;
            this.applicationDbContext = applicationDbContext;
            this.options = options;
        }

        public async Task Sync()
        {
            var service = await GetSheetService();

            var sheet = await GetSheet(service);

            string[] names = [
                nameof(ApplicationUser),
                nameof(Food),
                nameof(RecipeFood),
                nameof(Recipe),
                nameof(UserMeal),
                nameof(UserVital),
                nameof(UserGoal),
                nameof(UserSetting)
                ];

            await AddWorksheets(service, sheet, names);

            await SyncDbSet<Food>(service, sheet);
        }

        public async Task<Spreadsheet> GetSheet(SheetsService service)
        {
            var sheetRequest = service.Spreadsheets.Get(SpreadsheetId);

            sheetRequest.IncludeGridData = true;

            var sheet = await sheetRequest.ExecuteAsync();
            return sheet;
        }

        public async Task<SheetsService> GetSheetService()
        {
            using var stream = await fileService.GetLocalFileAsync(options.Value.FileName);
            var credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            return service;
        }

        public async Task<List<T>> GetData<T>(Spreadsheet sheet) where T : class
        {
            //var entityType = applicationDbContext.Model.FindEntityType(typeof(T)) ?? throw new Exception();

            //var properties = entityType.GetProperties();

            var name = typeof(T).Name;
            var sheetData = sheet.Sheets.First(s => s.Properties.Title == name).Data.First();

            var builder = new StringBuilder();

            foreach (var row in sheetData.RowData)
            {
                var rowValue = row.Values.Select(v => v.FormattedValue).ToArray();
                builder.AppendLine(string.Join(",", rowValue));
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.IgnoreReferences = true;

            using (var stream = new MemoryStream())
            using (var streamReader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(streamReader, config))
            {
                var str = builder.ToString();
                writer.Write(str);
                writer.Flush();

                stream.Position = 0;

                var result = csvReader.GetRecords<T>().ToList();
                return result;
            }

        }

        async Task SyncDbSet<T>(SheetsService service, Spreadsheet sheet) where T : class, IEntityId, IEntityAudit
        {
            var name = typeof(T).Name;
            var dbData = await applicationDbContext.Set<T>().ToListAsync();

            var sheetData = await GetData<T>(sheet);


            //add to db
            await UpdateDatabase(dbData, sheetData);

            var sheetDataToSend = sheetData.ToList();

            //update sheet
            var updatedSheetInfo = (
                from sheetItem in sheetData
                join dbRow in dbData on sheetItem.Id equals dbRow.Id
                select dbRow.DateModified > sheetItem.DateModified ? dbRow : sheetItem).ToList();

            sheetDataToSend.AddRange(updatedSheetInfo);

            //add to sheet
            var missingDbInfo = dbData.Where(d => !sheetData.Select(s => s.Id).Contains(d.Id)).ToList();
            sheetDataToSend.AddRange(missingDbInfo);

            sheetDataToSend.Sort((a,b) => a.Id < b.Id ? 1 : -1);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.IgnoreReferences = true;

            

            var range = new ValueRange();

            range.Values = new List<IList<object>>();

            var request = service.Spreadsheets.Values.Update(range, SpreadsheetId, $"{name}!A1");

            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            await request.ExecuteAsync();
        }

        private async Task UpdateDatabase<T>(List<T> dbData, List<T> sheetData) where T : class, IEntityId, IEntityAudit
        {
            var missingSheetInfo = sheetData.Where(s => !dbData.Select(d => d.Id).Contains(s.Id)).ToList();

            await applicationDbContext.AddRangeAsync(missingSheetInfo);

            var updatedSheetInfo = (
                from dbRow in dbData
                join sheet in sheetData on dbRow.Id equals sheet.Id
                where sheet.DateModified > dbRow.DateModified
                select sheet).ToList();

            applicationDbContext.UpdateRange(updatedSheetInfo);

            await applicationDbContext.SaveChangesAsync();
        }

        async Task AddWorksheets(SheetsService service, Spreadsheet sheets, string[] sheetNames)
        {
            var request = new BatchUpdateSpreadsheetRequest() { Requests = new List<Google.Apis.Sheets.v4.Data.Request>() };

            foreach (string sheetName in sheetNames)
            {
                if (sheets.Sheets.Any(s => s.Properties.Title == sheetName))
                    continue;


                request.Requests.Add(new Google.Apis.Sheets.v4.Data.Request { AddSheet = new AddSheetRequest() { Properties = new SheetProperties { Title = sheetName } } });

            }

            if (request.Requests.Count > 0)
            {
                await service.Spreadsheets.BatchUpdate(request, SpreadsheetId).ExecuteAsync();
            }
        }
    }
}
