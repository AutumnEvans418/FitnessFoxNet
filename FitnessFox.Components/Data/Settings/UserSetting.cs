using FitnessFox.Data;
using System.ComponentModel.DataAnnotations;

namespace FitnessFox.Components.Data.Settings
{
    public enum SettingKey
    {
        [Display(Name = "Google Spreadsheet Id")]
        SpreadsheetId,
        [Display(Name = "Sync on Start")]
        SyncOnStart
    }

    public class UserSetting : IEntityAudit, IEntityId<string>
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Value { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
