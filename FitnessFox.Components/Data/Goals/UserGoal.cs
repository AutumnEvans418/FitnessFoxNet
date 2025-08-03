namespace FitnessFox.Data.Goals
{
    public class UserGoal
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public UserGoalType Type { get; set; }
        public float Value { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }

}
