namespace FitnessFox.Data
{
    public enum UserGoalType
    {
        Weight,
        WeightLoss_Week,
        Bmi,
        Calories,
        Cholesterol,
        Sodium,
        VitaminK,
        Sugar,
        WorkoutMinutes_Week,
        LbsLifted_Week,
        CaloriesBurned_Week,
    }

    public class UserGoal
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
