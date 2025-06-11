namespace FitnessFox.Data
{
    public class RecipeFood
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int FoodId { get; set; }
        public float Amount { get; set; }
        public Recipe Recipe { get; set; } = null!;
        public Food Food { get; set; } = null!;
    }

}
