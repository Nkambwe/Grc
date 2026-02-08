using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class DepartmentFullUnitModel {
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
        [JsonPropertyName("unitHead")]
        public string UnitHead { get; set; }
        [JsonPropertyName("unitContactEmail")]
        public string UnitContactEmail { get; set; }
        [JsonPropertyName("unitContactNumber")]
        public string UnitContactNumber { get; set; }
        [JsonPropertyName("unitHeadDesignation")]
        public string UnitHeadDesignation { get; set; }
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }
        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }
        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }
    }

}