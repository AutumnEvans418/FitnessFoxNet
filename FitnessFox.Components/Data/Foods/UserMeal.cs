using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessFox.Data.Foods
{
    public class UserMeal : Nutrients, IEntityAudit, IEntityId<Guid>
    {
        public Guid Id { get; set; } 
        public string UserId { get; set; } = null!;
        public DateTime Date { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public UserMealType Type { get; set; }
        public float Servings { get; set; }
        public int? FoodId { get; set; }
        public int? RecipeId { get; set; }
        public Food? Food { get; set; }
        public Recipe? Recipe { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public string Name => Food?.Name ?? Recipe?.Name ?? "NO NAME";

        [NotMapped]
        public Nutrients? MealItem
        {
            get => (Nutrients?)Food ?? Recipe;
            set
            {
                if (value is Food food)
                {
                    Food = food;
                }
                else if (value is Recipe recipe)
                {
                    Recipe = recipe;
                }
                else if (value == null)
                {
                    Food = null;
                    Recipe = null;
                }
            }
        }


        public string ServingUnitDisplay
        {
            get
            {
                if (MealItem is Food food)
                {
                    return $"{MathF.Round(Servings*food.ServingSize, 2)} {food.ServingUnit}";
                }
                return string.Empty;
            }
        }

        public void SetNutrients()
        {
            Calories = MealItem?.Calories * Servings ?? 0;

        }

    }

}
