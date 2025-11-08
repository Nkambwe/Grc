using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ResponsibilityItemResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("responsibilityRole")]
        public string ResponsibilityRole { get; set; }
    }
}
