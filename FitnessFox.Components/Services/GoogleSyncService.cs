using FitnessFox.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.Services
{

    public class GoogleSyncService
    {
        private readonly IFileService fileService;
        private readonly ApplicationDbContext applicationDbContext;
        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        string ApplicationName = "FitnessFox";
        string SpreadsheetId = "1nrJfqZ-0mWHS9ChBthiyvPomOafk-46wepEVUm6g8hw";

        public GoogleSyncService(IFileService fileService, ApplicationDbContext applicationDbContext)
        {
            this.fileService = fileService;
            this.applicationDbContext = applicationDbContext;
        }
        public async Task Sync()
        {
            using var stream = await fileService.GetLocalFileAsync("fitnessfox-467923-9857f7a3ab7e.json");
            var credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var sheet = await service.Spreadsheets.Get(SpreadsheetId).ExecuteAsync();


        }


    }
}
