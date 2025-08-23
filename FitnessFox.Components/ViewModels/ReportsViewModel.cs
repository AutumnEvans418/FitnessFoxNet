using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Vitals;
using FitnessFox.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.ViewModels
{
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

        public DateTime? From { get; set; } = DateTime.Now.Date.AddDays(-6);
        public DateTime? To { get; set; } = DateTime.Now.Date;
        public List<ChartSeries> WeightSeries { get; set; } = [];

        public List<ChartSeries> HeartSeries { get; set; } = [];

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

            if (From >= To)
            {
                return;
            }

            WeightSeries = [];
            HeartSeries = [];
            Labels = new string[WeightSeries.Count];

            var vitals = await dbContext
                .UserVitals
                .Where(u => u.UserId == user.Id && u.Date >= From && u.Date <= To)
                .ToListAsync();

            var series = CreateSeries(vitals, From, To, [UserVitalType.Weight]);

            WeightSeries = series.series;
            Labels = series.labels;

            var series2 = CreateSeries(vitals, From, To, [UserVitalType.Systolic, UserVitalType.Diastolic, UserVitalType.Bpm]);

            HeartSeries = series2.series;

            var goal = await dbContext
                .UserGoals
                .Where(u => u.UserId == user.Id && u.Type == FitnessFox.Data.Goals.UserGoalType.WeightLbs)
                .FirstOrDefaultAsync();

            if (goal != null)
            {
                var name = goal.Type.GetAttribute<DisplayAttribute>()?.Name ?? goal.Type.ToString();
                WeightSeries.Add(new ChartSeries
                {
                    Name = name,
                    Data = Enumerable.Range(0, Labels.Length).Select(d => (double)goal.Value).ToArray()
                });
            }
        }

        private static (List<ChartSeries> series, string[] labels) CreateSeries(
            List<UserVital> vitals, 
            DateTime? from,
            DateTime? to,
            UserVitalType[] vitalsToDisplay)
        {
            List<ChartSeries> chartSeries = [];

            var filtered = vitals.Where(v => vitalsToDisplay.Contains(v.Type)).GroupBy(g => g.Date.Date).OrderBy(g => g.Key).ToList();

            string[] labels = filtered.Select(s => s.Key.ToString("MMM d")).ToArray();

            

            foreach (var type in vitalsToDisplay)
            {
                var series = new ChartSeries();
                chartSeries.Add(series);

                series.Data = new double[labels.Length];
                series.Name = type.ToString();
                series.ShowDataMarkers = true;

                for (int i = 0; i < filtered.Count; i++)
                {
                    var item = filtered[i];
                    var data = item.Where(i => i.Type == type).Select(i => i.Value).DefaultIfEmpty().Average();
                    series.Data[i] = data;
                }
            }

            return (chartSeries, labels);
            //var days = (int)((to - from)?.TotalDays ?? 0) + 1;

            //var binSize = Math.Max(1, days / 7);

            //var labels = new string[days / binSize];
            //List<ChartSeries> chartSeries = [];

            //foreach (var vitalType in vitalsToDisplay)
            //{
            //    var series = new ChartSeries();
            //    chartSeries.Add(series);
            //    var data = new double[days / binSize];

            //    series.Data = data;
            //    series.Name = vitalType.ToString();
            //    series.ShowDataMarkers = true;

            //    var bin = 0;
            //    var value = 0f;

            //    for (int i = 0; i < days; i++)
            //    {
            //        var date = from.GetValueOrDefault().AddDays(i);
            //        var userVital = vitals.FirstOrDefault(v => v.Type == vitalType && v.Date.Date == date.Date);

            //        if (userVital != null)
            //        {
            //            value += userVital.Value;
            //        }

            //        if ((i + 1) % binSize == 0)
            //        {
            //            labels[bin] = date.ToString("MMM d");
            //            data[bin] = value / binSize;
            //            value = 0f;
            //            bin++;
            //        }
            //    }
            //}

            //return (chartSeries, labels);
        }
    }
}
