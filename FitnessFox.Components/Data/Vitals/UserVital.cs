namespace FitnessFox.Data.Vitals
{
    public class UserVital : IEntityAudit, IEntityId<int>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime Date { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public UserVitalType Type { get; set; }
        public float Value { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
