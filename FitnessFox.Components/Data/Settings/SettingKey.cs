using System.ComponentModel.DataAnnotations;

namespace FitnessFox.Components.Data.Settings
{
    public enum SettingKey
    {
        [Display(Name = "Google Spreadsheet Id", ResourceType = typeof(string))]
        SpreadsheetId,
        [Display(Name = "Sync on Start", ResourceType = typeof(bool))]
        SyncOnStart,
        SyncLastDate,
    }
}
