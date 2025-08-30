using System.ComponentModel.DataAnnotations;

namespace FitnessFox.Data.Vitals
{
    public enum UserVitalType
    {
        Water,
        Weight,
        Temperature,
        Systolic,
        Diastolic,
        Bpm,
        [Display(Name = "Waist (In)")]
        WaistIn,
        [Display(Name = "Underbust (In)")]
        UnderbustIn,
        [Display(Name = "Standing Bust (In)")]
        StandingBustIn,
        [Display(Name = "Leaning Bust (In)")]
        LeaningBustIn,
    }
}
