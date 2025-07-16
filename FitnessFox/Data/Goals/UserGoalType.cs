using System.ComponentModel.DataAnnotations;

namespace FitnessFox.Data.Goals
{
    public enum UserGoalType
    {
        [Display(Name = "Weight Goal (Lbs)")]
        WeightLbs,
        [Display(Name = "Weight Loss (Lbs) per Week (ex 1-2 lbs)")]
        WeightLossLbs_Week,
        [Display(Name = "BMI Goal")]
        Bmi,
        [Display(Name = "Daily Calories (ex. 2000")]
        Calories,
        [Display(Name = "Daily Cholesterol (mg) (ex 200)")]
        Cholesterol,
        [Display(Name = "Daily Sodium (mg) (ex 2000)")]
        Sodium,
        [Display(Name = "Daily VitaminK (mcg) (ex 200)")]
        VitaminK,
        [Display(Name = "Daily Sugar (mg) (ex 36)")]
        Sugar,
        [Display(Name = "Workout Minutes per Week (ex 90)")]
        WorkoutMinutes_Week,
        [Display(Name = "Lbs lifed per Week (ex 1000)")]
        LbsLifted_Week,
        [Display(Name = "Calories burned per Week (ex 300)")]
        CaloriesBurned_Week,
    }
}
