namespace FitnessFox.Data
{
    public class UserSetting : IEntityAudit, IEntityId<string>
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Value { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
