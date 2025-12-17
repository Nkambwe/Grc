using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class DocumentTypeResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("typeName")]
        public string TypeName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedAt { get; set; }
    }

}
