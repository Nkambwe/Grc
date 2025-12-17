using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class PolicyDepartmentResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }
    }
}
