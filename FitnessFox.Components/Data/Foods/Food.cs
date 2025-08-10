using Microsoft.VisualBasic;

namespace FitnessFox.Data.Foods
{
    public class Food : Nutrients, IEntityId<int>, IEntityAudit
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string BrandRestaurant { get; set; } = null!;
        public string Description { get; set; } = null!;
        public float ServingSize { get; set; }
        public string ServingUnit { get; set; } = null!;
        public float TotalServings { get; set; } = 1;

        public ApplicationUser User { get; set; } = null!;

        public string Name => $"{BrandRestaurant} - {Description}";

        public override string ToString()
        {
            return Name;
        }

    }

}
