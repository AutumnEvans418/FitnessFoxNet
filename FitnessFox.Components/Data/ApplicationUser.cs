
namespace FitnessFox.Data
{
    public class ApplicationUser : IEntityAudit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserName { get; set; }
        public float HeightInches { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public float HeightDisplayFeet => MathF.Floor(HeightInches / 12f);
        public float HeightDisplayInches => HeightInches % 12f;

    }
}
