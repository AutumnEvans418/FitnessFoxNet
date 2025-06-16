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

        public void SetNutrients()
        {
            if (Foods.Count <= 0)
                return;

            var aggregate = Foods.Aggregate(new Food(), (first, second) => new Food
            {
                Calories = first.Calories + (second.Amount * second.Food?.Calories ?? 0),
            });

            if (NumberOfPeople == 0 || aggregate == null)
                return;

            Calories = aggregate.Calories / NumberOfPeople;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
