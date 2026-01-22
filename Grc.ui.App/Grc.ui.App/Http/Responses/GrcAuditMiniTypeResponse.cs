using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcAuditMiniTypeResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

    }

}
