using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class DepartmentResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("branchId")]
        public long BranchId { get; set; }
        [JsonPropertyName("branch")]
        public string Branch { get; set; }
        [JsonPropertyName("departmentCode")]
        public string DepartmentCode { get; set; }
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }
        [JsonPropertyName("departmentAlias")]
        public string DepartmentAlias { get; set; }
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        [JsonPropertyName("createdOn")]
        public DateTime CreatdOn { get; set; } = DateTime.Now;

        [JsonPropertyName("departmentUnits")]
        public List<DepartmentUnitResponse> DepartmentUnits { get; set; } = new List<DepartmentUnitResponse>();
    }
}
