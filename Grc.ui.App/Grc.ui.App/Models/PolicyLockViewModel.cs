using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class PolicyLockViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isLocked")]
        public bool IsLocked { get; set; }

    }

}
