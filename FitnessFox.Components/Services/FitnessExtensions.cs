using System.Reflection;

namespace FitnessFox.Services
{
    public static class FitnessExtensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute? GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static float Bmi(float weight, float height) => MathF.Round((703 * weight) / MathF.Pow(height, 2), 2);
        public static float BmiWeight(float bmi, float height) => MathF.Round((bmi * MathF.Pow(height, 2)) / 703, 2);

        public static float Calories(float weightLoss, float caloriesBurnedWeekly) => 2000 - (weightLoss * 500) - (caloriesBurnedWeekly / 7);
    }
}
