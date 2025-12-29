using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ControlCategoryResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("items")]
        public List<ControlItemResponse> ControlItems { get; set; } = new();
    }
}
