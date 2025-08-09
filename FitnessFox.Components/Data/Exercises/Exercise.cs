
namespace FitnessFox.Data.Exercises
{
    public class Exercise : IEntityId, IEntityAudit
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ExerciseType Type { get; set; }
        public string Description { get; set; } = null!;
        public float? CaloriesPerMin { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified {  get; set; }
    }
}
