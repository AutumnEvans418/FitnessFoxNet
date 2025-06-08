using Microsoft.AspNetCore.Identity;

namespace FitnessFox.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public float HeightInches { get; set; }

        public float HeightDisplayFeet => MathF.Floor(HeightInches / 12f);
        public float HeightDisplayInches => HeightInches % 12f;

    }
}
