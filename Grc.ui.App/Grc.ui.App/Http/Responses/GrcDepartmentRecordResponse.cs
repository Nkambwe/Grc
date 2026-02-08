using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcDepartmentRecordResponse {
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
        [JsonPropertyName("headFullName")]
        public string HeadFullName { get; set; }
        [JsonPropertyName("headEmail")]
        public string HeadEmail { get; set; }
        [JsonPropertyName("headContact")]
        public string HeadContact { get; set; }
        [JsonPropertyName("headDesignation")]
        public string HeadDesignation { get; set; }
        [JsonPropertyName("headComment")]
        public string HeadComment { get; set; }
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; } = false;
        [JsonPropertyName("createdOn")]
        public DateTime CreatdOn { get; set; } = DateTime.Now;

        [JsonPropertyName("departmentUnits")]
        public List<GrcDepartmentUnitResponse> DepartmentUnits { get; set; } = new List<GrcDepartmentUnitResponse>();
    }
}
