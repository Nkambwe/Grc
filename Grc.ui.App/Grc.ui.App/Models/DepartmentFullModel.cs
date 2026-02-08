using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class DepartmentFullModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("departmentAlias")]
        public string DepartmentAlias { get; set; }="";
        [JsonPropertyName("departmentCode")]
        public string DepartmentCode { get; set; }="";
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }="";
        [JsonPropertyName("departmentHead")]
        public string DepartmentHead { get; set; }="";
        [JsonPropertyName("departmentHeadEmail")]
        public string DepartmentHeadEmail { get; set; }="";
        [JsonPropertyName("departmentHeadContact")]
        public string DepartmentHeadContact { get; set; }="";
        [JsonPropertyName("departmentHeadDesignation")]
        public string DepartmentHeadDesignation { get; set; }="";
    }

}