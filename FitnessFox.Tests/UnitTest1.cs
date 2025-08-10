using CsvHelper;
using CsvHelper.Configuration;
using FitnessFox.Components.Services;
using FitnessFox.Data.Foods;
using FluentAssertions;
using System.Data;
using System.Globalization;
using Xunit;

namespace FitnessFox.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CsvShouldCreateHeader()
        {
            using var stream = new MemoryStream();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            config.IgnoreReferences = true;

            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(new List<Food>());
            }

            var result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

            result.Should().StartWith("Id");
            result.Should().NotContain("UserName");
            result.Should().NotContain("User,");
        }
    }

    public class OcrServiceTest
    {
        [Fact]
        public async Task LabelShouldHaveCalories()
        {
            var service = new OcrService();

            var result = await service.ParseText("C:\\Users\\autumn\\source\\repos\\FitnessFox\\FitnessFox.Tests\\data\\20250809_164658.jpg");

            result.Should().Contain("Calories");
        }
    }
}