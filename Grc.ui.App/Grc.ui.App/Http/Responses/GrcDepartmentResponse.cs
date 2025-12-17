using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcDepartmentResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }
    }
}
