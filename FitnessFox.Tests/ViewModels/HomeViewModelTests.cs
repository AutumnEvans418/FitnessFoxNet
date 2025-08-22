using FitnessFox.Components.ViewModels;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Tests.ViewModels
{
    public class HomeViewModelTests : DbTestBase<HomeViewModel>
    {
        [Fact]
        public void Db_Should_HaveNoWeight()
        {
            Db.UserVitals.Should().BeEmpty();
        }

        [Fact]
        public async Task Save_Should_AddWeight()
        {
            await Subject.OnInitializedAsync();

            Subject.Weight.Value += 1;

            await Subject.Save(Subject.Weight);

            Db.UserVitals.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Save_Should_UpdateWeight()
        {
            var user = await AuthenticationService.GetUserAsync();
            Db.UserVitals.Add(new Data.Vitals.UserVital
            {
                User = user!,
                Type = Data.Vitals.UserVitalType.Weight,
                Date = Subject.CurrentDate.GetValueOrDefault(),
            });
            Db.SaveChanges();

            await Subject.OnInitializedAsync();

            Subject.Weight.Value += 1;

            await Subject.Save(Subject.Weight);

            Db.UserVitals.First().Value.Should().Be(1);
        }
    }
}
