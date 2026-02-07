using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcUnitItemResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("unitName")]
        public string UnitName { get; set; }
    }

    public class GrcDepartmentUnitResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }
        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }
        [JsonPropertyName("unitName")]
        public string UnitName { get; set; }
        [JsonPropertyName("department")]
        public string Department { get; set; }
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
        [JsonPropertyName("creatdOn")]
        public DateTime CreatdOn { get; set; }
        
    }
}
