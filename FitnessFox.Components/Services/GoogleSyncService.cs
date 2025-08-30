using CsvHelper;
using CsvHelper.Configuration;
using FitnessFox.Components.Data.Options;
using FitnessFox.Components.Data.Settings;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
using FitnessFox.Data.Vitals;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FitnessFox.Components.Services
{
    public class GoogleSyncService : ISyncService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IGoogleSheetsServices googleSheetsServices;
        private readonly IAuthenticationService authenticationService;

        public GoogleSyncService(
            ApplicationDbContext applicationDbContext,
            IGoogleSheetsServices googleSheetsServices,
            IAuthenticationService authenticationService)
        {
            this.applicationDbContext = applicationDbContext;
            this.googleSheetsServices = googleSheetsServices;
            this.authenticationService = authenticationService;
        }

        public async Task Sync()
        {
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

            await googleSheetsServices.LoadSheet();

            await googleSheetsServices.AddWorksheets(names);

            var user = await authenticationService.GetUserAsync();

            if (user == null)
                return;

            //await SyncDbSet<ApplicationUser, string>(a => a.Id == user.Id, a => { });
            await SyncDbSet<Food, Guid>(f => f.UserId == user.Id, a => a.UserId = user.Id);
            await SyncDbSet<RecipeFood, Guid>(rf => rf.Recipe!.UserId == user.Id, a => a.Recipe!.UserId = user.Id);
            await SyncDbSet<Recipe, Guid>(r => r.UserId == user.Id, a => a.UserId = user.Id);
            await SyncDbSet<UserMeal, Guid>(u => u.UserId == user.Id, a => a.UserId = user.Id);
            await SyncDbSet<UserVital, Guid>(u => u.UserId == user.Id, a => a.UserId = user.Id);
            await SyncDbSet<UserGoal, Guid>(u => u.UserId == user.Id, a => a.UserId = user.Id);
            await SyncDbSet<UserSetting, string>(u => u.UserId == user.Id, a => a.UserId = user.Id);
        }

        public async Task<List<T>> GetData<T>() where T : class
        {
            var name = typeof(T).Name;
            var sheetData = await googleSheetsServices.GetSheetRows(name);

            if (sheetData == null || sheetData.Count == 0)
                return [];

            var builder = new StringBuilder();

            foreach (var row in sheetData)
            {
                if (row.All(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                builder.AppendLine(string.Join(",", row));
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.IgnoreReferences = true;
            config.HeaderValidated = null;
            config.MissingFieldFound = null;

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

        public async Task SyncDbSet<T, S>(Expression<Func<T, bool>> filter, Action<T> action) where T : class, IEntityId<S>, IEntityAudit
        {
            var dbData = await applicationDbContext.Set<T>().Where(filter).ToListAsync();

            var sheetData = await GetData<T>();

            await UpdateSheet<T, S>(dbData, sheetData);
            await UpdateDatabase<T, S>(dbData, sheetData, action);
        }

        private async Task UpdateSheet<T, S>(List<T> dbData, List<T> sheetData) where T : class, IEntityId<S>, IEntityAudit
        {
            var name = typeof(T).Name;

            //Sheet data that was not in Db.
            var sheetDataToSend = sheetData.Where(s => !dbData.Select(d => d.Id).Contains(s.Id)).ToList();

            //Sheet data with matching records that hasn't changed.
            var upToDateSheetInfo = (
                from sheetItem in sheetData
                join dbRow in dbData on sheetItem.Id equals dbRow.Id
                where (dbRow.DateModified - sheetItem.DateModified) <= TimeSpan.FromSeconds(1)
                select dbRow).ToList();

            sheetDataToSend.AddRange(upToDateSheetInfo);

            //Sheet data with matching records where the db is newer
            var updatedSheetInfo = (
                from sheetItem in sheetData
                join dbRow in dbData on sheetItem.Id equals dbRow.Id
                where (dbRow.DateModified - sheetItem.DateModified) > TimeSpan.FromSeconds(1)
                select dbRow).ToList();

            sheetDataToSend.AddRange(updatedSheetInfo);



            //Db data that was not in the sheet
            var missingDbInfo = dbData.Where(d => !sheetData.Select(s => s.Id).Contains(d.Id)).ToList();
            sheetDataToSend.AddRange(missingDbInfo);

            if (updatedSheetInfo.Count == 0 && missingDbInfo.Count == 0)
            {
                return;
            }

            var entityType = applicationDbContext.Model.FindEntityType(typeof(T)) ?? throw new Exception();

            var properties = entityType.GetProperties();

            var range = new ValueRange
            {
                Values = []
            };


            var header = new List<object>();
            //create header
            foreach (var item in properties)
            {
                header.Add(item.Name);
            }

            range.Values.Add(header);

            var type = typeof(T);

            //rows
            foreach (var item in sheetDataToSend)
            {
                var row = new List<object?>();

                foreach (var property in properties)
                {
                    row.Add(type.GetProperty(property.Name)?.GetValue(item)?.ToString());
                }

                range.Values.Add(row);
            }

            await googleSheetsServices.UpdateSheet($"{name}!A1", range.Values);
        }

        private async Task UpdateDatabase<T, S>(List<T> dbData, List<T> sheetData, Action<T> action) where T : class, IEntityId<S>, IEntityAudit
        {
            var missingSheetInfo = sheetData.Where(s => !dbData.Select(d => d.Id).Contains(s.Id)).ToList();

            foreach (var item in missingSheetInfo)
            {
                action(item);
                applicationDbContext.Add(item);
            }

            var updatedSheetInfo = (
                from dbRow in dbData
                join sheet in sheetData on dbRow.Id equals sheet.Id
                where (sheet.DateModified - dbRow.DateModified) > TimeSpan.FromSeconds(1)
                select new { From = dbRow, To = sheet }).ToList();

            foreach (var item in updatedSheetInfo)
            {
                applicationDbContext.Entry(item.From).CurrentValues.SetValues(item.To);
                action(item.From);
                applicationDbContext.Update(item.From);
            }

            await applicationDbContext.SaveChangesAsync();
        }

        

    }
}
