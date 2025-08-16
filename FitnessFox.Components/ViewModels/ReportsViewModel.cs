using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Vitals;
using FitnessFox.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.ViewModels
{
    public enum TimeFrame
    {
        Week,
        Month,
        Year,
        All
    }

    public class ReportsViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ApplicationDbContext dbContext;

        public ReportsViewModel(
            IDialogService mudDialogInstance,
            ILoggingService loggingService,
            ILoadingService loadingService,
            IAuthenticationService authenticationService,
            ApplicationDbContext dbContext)
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.authenticationService = authenticationService;
            this.dbContext = dbContext;
        }

        public DateTime? From { get; set; } = DateTime.Now.Date.AddDays(-7);
        public DateTime? To { get; set; } = DateTime.Now.Date;
        public List<ChartSeries> Series { get; set; } = [];
        public string[] Labels { get; set; } = [];

        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        public async Task Refresh()
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
            {
                return;
            }

            if(From >= To)
            {
                return;
            }

            var vitals = await dbContext
                .UserVitals
                .Where(u => u.UserId == user.Id && u.Date >= From && u.Date <= To)
                .ToListAsync();

            var days = (int)((To - From)?.TotalDays ?? 0);

            var vitalsToDisplay = new[] { UserVitalType.Weight };

            Labels = new string[days];
            Series = [];
            foreach (var vitalType in vitalsToDisplay)
            {
                var series = new ChartSeries();
                Series.Add(series);
                var data = new double[days];

                series.Data = data;
                series.Name = vitalType.ToString();
                for (int i = 0; i < days; i++)
                {
                    var date = From.GetValueOrDefault().AddDays(i+1);

                    Labels[i] = date.ToString("M");

                    var userVital = vitals.FirstOrDefault(v => v.Type == vitalType && v.Date.Date == date.Date);

                    if (userVital != null)
                    {
                        data[i] = userVital.Value;
                    }
                }
            }
        }
    }
}
