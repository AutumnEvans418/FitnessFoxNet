using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
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
    public enum ChartDataType
    {
        Weight,
        Blood,
        Figure,
        Food,
    }

    public class ChartViewModel
    {
        public ChartDataType Name { get; set; }
        public List<ChartSeries> Series { get; set; } = new();
        public string[] Labels { get; set; } = [];
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

        public DateTime? From { get; set; } = DateTime.Now.Date.AddDays(-29);
        public DateTime? To { get; set; } = DateTime.Now.Date;

        public List<ChartViewModel> Charts { get; set; } = new();

        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        public List<UserVital> Vitals { get; set; } = new();
        public List<UserGoal> Goals { get; set; } = new();
        public List<UserMeal> Meals { get; set; } = new();
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

            Charts.Clear();

            var dateOnlyFrom = DateOnly.FromDateTime(From.GetValueOrDefault());
            var dateOnlyTo = DateOnly.FromDateTime(To.GetValueOrDefault());

            Vitals = await dbContext
                .UserVitals
                .Where(u => u.UserId == user.Id && u.Date >= dateOnlyFrom && u.Date <= dateOnlyTo)
                .ToListAsync();

            Goals = await dbContext
                .UserGoals
                .Where(u => u.UserId == user.Id)
                .ToListAsync();

            Meals = await dbContext
                .UserMeals
                .Where(u => u.UserId == user.Id && u.Date >= From && u.Date <= To)
                .ToListAsync();

            CreateSeries(ChartDataType.Weight, [UserVitalType.Weight], [UserGoalType.WeightLbs]);
            CreateSeries(ChartDataType.Blood, [UserVitalType.Systolic, UserVitalType.Diastolic, UserVitalType.Bpm], []);
            CreateSeries(ChartDataType.Figure, [UserVitalType.WaistIn, UserVitalType.UnderbustIn, UserVitalType.StandingBustIn, UserVitalType.LeaningBustIn], []);

            CreateFoodSeries(ChartDataType.Food, u => 
                [(u.Calories, "Calories"), (u.Cholesterol, "Cholesterol"), (u.Sugar, "Sugar"), (u.Sodium, "Sodium"), (u.VitaminK, "VitaminK")], 
                [UserGoalType.Calories, UserGoalType.Cholesterol, UserGoalType.Sugar, UserGoalType.Sodium, UserGoalType.VitaminK]);
        }

        private void CreateFoodSeries(ChartDataType chartName, Func<UserMeal, (float, string)[]> props, UserGoalType[] goalTypes)
        {
            var filtered = Meals.GroupBy(g => g.Date.Date).OrderBy(g => g.Key).ToList();

            if (filtered.Count == 0)
                return;

            var binSize = Math.Max(1, filtered.Count / 7);

            string[] labels = filtered.Select((s, i) => i % binSize == 0 ? s.Key.ToString("MMM d") : "").ToArray();

            var count = props(filtered.First().First()).Length;

            ChartSeries[] chartSeries = new ChartSeries[count];

            for (int i = 0; i < filtered.Count; i++)
            {
                var item = filtered[i].Select(props);

                for (int j = 0; j < count; j++)
                {
                    chartSeries[j] ??= new ChartSeries()
                    {
                        Data = new double[labels.Length],
                        Name = item.FirstOrDefault()?[j].Item2 ?? "NA",
                        ShowDataMarkers = true,
                    };
                    var data = item.DefaultIfEmpty().Average(p => p?[j].Item1 ?? 0);
                    chartSeries[j].Data[i] = data;
                }
            }

            AddGoals(goalTypes, chartSeries.ToList(), labels);

            Charts.Add(new ChartViewModel
            {
                Labels = labels,
                Series = chartSeries.ToList(),
                Name = chartName,
            });
        }

        private void CreateSeries(
            ChartDataType chartName,
            UserVitalType[] vitalsToDisplay,
            UserGoalType[] goalTypes)
        {
            List<ChartSeries> chartSeries = [];

            var filtered = Vitals.Where(v => vitalsToDisplay.Contains(v.Type)).GroupBy(g => g.Date).OrderBy(g => g.Key).ToList();

            var binSize = Math.Max(1, filtered.Count / 7);

            string[] labels = filtered.Select((s, i) => i % binSize == 0 ? s.Key.ToString("MMM d") : "").ToArray();

            foreach (var type in vitalsToDisplay)
            {
                var series = new ChartSeries();
                chartSeries.Add(series);
                var name = type.GetAttribute<DisplayAttribute>()?.Name ?? type.ToString();

                series.Data = new double[labels.Length];
                series.Name = name;
                series.ShowDataMarkers = true;

                for (int i = 0; i < filtered.Count; i++)
                {
                    var item = filtered[i];
                    var data = item.Where(i => i.Type == type).Select(i => i.Value).DefaultIfEmpty().Average();
                    series.Data[i] = data;
                }
            }

            AddGoals(goalTypes, chartSeries, labels);

            Charts.Add(new ChartViewModel
            {
                Labels = labels,
                Series = chartSeries,
                Name = chartName,
            });
        }

        private void AddGoals(UserGoalType[] goalTypes, List<ChartSeries> chartSeries, string[] labels)
        {
            var goals = Goals.Where(g => goalTypes.Contains(g.Type)).ToList();

            foreach (var goal in goals)
            {
                if (goal != null)
                {
                    var name = goal.Type.GetAttribute<DisplayAttribute>()?.Name ?? goal.Type.ToString();
                    chartSeries.Add(new ChartSeries
                    {
                        Name = name,
                        Data = Enumerable.Range(0, labels.Length).Select(d => (double)goal.Value).ToArray()
                    });
                }
            }
        }
    }
}
