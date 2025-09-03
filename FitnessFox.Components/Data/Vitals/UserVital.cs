namespace FitnessFox.Data.Vitals
{
    public class UserVital : IEntityAudit, IEntityId<Guid>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateOnly Date { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public UserVitalType Type { get; set; }
        public float Value { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
