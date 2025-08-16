using FitnessFox.Components.Services;
using FluentAssertions;

namespace FitnessFox.Tests
{
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