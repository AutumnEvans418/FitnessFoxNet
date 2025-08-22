namespace FitnessFox.Data.Foods
{
    public class RecipeFood : IEntityId<Guid>, IEntityAudit
    {
        public Guid Id { get; set; }
        public int RecipeId { get; set; }
        public int FoodId { get; set; }
        public float Amount { get; set; }
        public Recipe? Recipe { get; set; }
        public Food? Food { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string ServingUnitDisplay => $"{MathF.Round(Amount * Food?.ServingSize ?? 0, 2)} {Food?.ServingUnit}";

    }

}
