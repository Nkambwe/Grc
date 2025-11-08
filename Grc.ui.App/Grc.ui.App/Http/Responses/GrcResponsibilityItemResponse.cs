using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcResponsibilityItemResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("responsibilityRole")]
        public string ResponsibilityRole { get; set; }
    }
}
