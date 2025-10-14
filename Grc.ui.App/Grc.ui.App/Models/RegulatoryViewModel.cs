using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class RegulatoryViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
    }
}
