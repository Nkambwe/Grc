using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class UserByIdRequest : GrcRequest {

        [JsonPropertyName("recordId")]
        public long RecordId { get; set; }
    }
}
