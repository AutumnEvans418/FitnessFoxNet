namespace FitnessFox.Data
{
    public class UserMeal : Nutrients
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public UserMealType Type { get; set; }
        public float Servings { get; set; }
        public int? FoodId { get; set; }
        public int? RecipeId { get; set; }
        public Food? Food { get; set; }
        public Recipe? Recipe { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }

}
