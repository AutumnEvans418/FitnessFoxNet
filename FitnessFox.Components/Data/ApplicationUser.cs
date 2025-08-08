
namespace FitnessFox.Data
{
    public class ApplicationUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserName { get; set; }
        public float HeightInches { get; set; }

        public float HeightDisplayFeet => MathF.Floor(HeightInches / 12f);
        public float HeightDisplayInches => HeightInches % 12f;

    }
}
