namespace FitnessFox.Data
{
    public class UserSetting
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}
