using FitnessFox.Components.Data.Options;
using FitnessFox.Components.Data.Settings;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Options;

namespace FitnessFox.Components.Services
{
    public class GoogleSheetsServices : IGoogleSheetsServices
    {
        private readonly IOptions<GoogleOptions> options;
        private readonly ISettingsService settingsService;
        private readonly IFileService fileService;
        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        string ApplicationName = "FitnessFox";
        SheetsService? service;
        Spreadsheet? sheets;

        public GoogleSheetsServices(
            IOptions<GoogleOptions> options,
            ISettingsService settingsService,
            IFileService fileService)
        {
            this.options = options;
            this.settingsService = settingsService;
            this.fileService = fileService;
        }

        public async Task AddWorksheets(string[] sheetNames)
        {
            service ??= await GetSheetService();
            sheets ??= await GetSheet(service);
            if (sheets == null)
                return;


            var request = new BatchUpdateSpreadsheetRequest() { Requests = new List<Google.Apis.Sheets.v4.Data.Request>() };

            foreach (string sheetName in sheetNames)
            {
                if (sheets.Sheets.Any(s => s.Properties.Title == sheetName))
                    continue;


                request.Requests.Add(new Google.Apis.Sheets.v4.Data.Request { AddSheet = new AddSheetRequest() { Properties = new SheetProperties { Title = sheetName } } });

            }

            if (request.Requests.Count > 0)
            {
                var spreadSheetId = await settingsService.GetValue<string?>(SettingKey.SpreadsheetId);

                await service.Spreadsheets.BatchUpdate(request, spreadSheetId).ExecuteAsync();
            }
        }

        public async Task UpdateSheet(string range, IList<IList<object>> rows)
        {
            service ??= await GetSheetService();
            sheets ??= await GetSheet(service);
            if (sheets == null) return;

            var spreadSheetId = await settingsService.GetValue<string?>(SettingKey.SpreadsheetId);

            var body = new ValueRange
            {
                Values = rows,
            };

            var request = service.Spreadsheets.Values.Update(body, spreadSheetId, range);

            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            await request.ExecuteAsync();
        }

        public async Task<List<List<string>>> GetSheetRows(string name)
        {
            service ??= await GetSheetService();
            sheets ??= await GetSheet(service);
            if (sheets == null)
                return [];

            var sheetData = sheets.Sheets.First(s => s.Properties.Title == name).Data.First();

            if (sheetData.RowData == null)
                return [];

            return sheetData.RowData.Select(row => row.Values.Select(v => v.FormattedValue).ToList()).ToList();
        }


        private async Task<SheetsService> GetSheetService()
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

        private async Task<Spreadsheet?> GetSheet(SheetsService service)
        {
            var spreadSheetId = await settingsService.GetValue<string?>(SettingKey.SpreadsheetId);

            if (string.IsNullOrEmpty(spreadSheetId))
            {
                return null;
            }

            var sheetRequest = service.Spreadsheets.Get(spreadSheetId);

            sheetRequest.IncludeGridData = true;

            var sheet = await sheetRequest.ExecuteAsync();
            return sheet;
        }




    }
}
