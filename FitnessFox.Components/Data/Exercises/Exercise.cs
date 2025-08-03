namespace FitnessFox.Data.Exercises
{
    public class Exercise
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ExerciseType Type { get; set; }
        public string Description { get; set; } = null!;
        public float? CaloriesPerMin { get; set; }
    }
}
