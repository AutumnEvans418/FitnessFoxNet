using FitnessFox.Data;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FitnessFox.Components.Data.Settings
{
    public class UserSetting : IEntityAudit, IEntityId<string>
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Value { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public T? GetValue<T>()
        {
            if (Value == null)
                return default;

            return JsonConvert.DeserializeObject<T?>(Value);
        }
    }
}
