using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class  DepartmentUnitResponse {
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
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = "";
        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }
        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }
    }
}
