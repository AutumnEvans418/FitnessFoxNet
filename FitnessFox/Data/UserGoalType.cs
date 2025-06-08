using System.ComponentModel.DataAnnotations;

namespace FitnessFox.Data
{
    public enum UserGoalType
    {
        [Display(Name = "Weight Goal (Lbs)")]
        WeightLbs,
        [Display(Name = "Weight Loss (Lbs) per Week")]
        WeightLossLbs_Week,
        [Display(Name = "BMI Goal")]
        Bmi,
        [Display(Name = "Daily Calories")]
        Calories,
        [Display(Name = "Daily Cholesterol")]
        Cholesterol,
        [Display(Name = "Daily Sodium")]
        Sodium,
        [Display(Name = "Daily VitaminK")]
        VitaminK,
        [Display(Name = "Daily Sugar")]
        Sugar,
        [Display(Name = "Workout Minutes per Week")]
        WorkoutMinutes_Week,
        [Display(Name = "Lbs lifed per Week")]
        LbsLifted_Week,
        [Display(Name = "Calories burned per Week")]
        CaloriesBurned_Week,
    }
}
