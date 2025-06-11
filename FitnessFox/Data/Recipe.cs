namespace FitnessFox.Data
{
    public class Recipe : Nutrients
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Name { get; set; } = null!;
        public float NumberOfPeople { get; set; } = 1;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public List<RecipeFood> Foods { get; set; } = [];
    }

}
