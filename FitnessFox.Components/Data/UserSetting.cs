namespace FitnessFox.Data
{
    public class UserSetting : IEntityAudit, IEntityId
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
