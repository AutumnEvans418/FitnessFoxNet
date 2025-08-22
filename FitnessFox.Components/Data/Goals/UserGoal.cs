namespace FitnessFox.Data.Goals
{
    public class UserGoal : IEntityId<Guid>, IEntityAudit
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public UserGoalType Type { get; set; }
        public float Value { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }

}
