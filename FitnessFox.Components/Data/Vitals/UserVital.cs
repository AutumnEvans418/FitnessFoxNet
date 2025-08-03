namespace FitnessFox.Data.Vitals
{
    public class UserVital
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public UserVitalType Type { get; set; }
        public float Value { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
