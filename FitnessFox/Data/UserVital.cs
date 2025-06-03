namespace FitnessFox.Data
{
    public class UserVital
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public UserVitalType Type { get; set; }
        public float Value { get; set; }
        public string? Notes { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
