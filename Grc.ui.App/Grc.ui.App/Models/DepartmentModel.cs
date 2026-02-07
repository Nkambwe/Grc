using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class DepartmentModel {
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
        public DateTime CreatedOn { get; set; }
    }

}