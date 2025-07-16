namespace FitnessFox.Data.Exercises
{
    public class UserExercise
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ExerciseId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public float? Minutes { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public float? Weight { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Exercise Exercise { get; set; } = null!;
    }
}
