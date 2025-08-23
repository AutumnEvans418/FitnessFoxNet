using FitnessFox.Components.Services;
using FluentAssertions;

namespace FitnessFox.Tests.Services
{
    public class OcrServiceTest
    {
        [Fact(Skip = "not ready yet")]
        public async Task LabelShouldHaveCalories()
        {
            var service = new OcrService();

            var result = await service.ParseText("C:\\Users\\autumn\\source\\repos\\FitnessFox\\FitnessFox.Tests\\data\\20250809_164658.jpg");

            result.Should().Contain("Calories");
        }
    }
}