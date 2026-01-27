using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class UserViewModel {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }=string.Empty;
        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }=string.Empty;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }=string.Empty;
        [JsonPropertyName("userName")]
        public string UserName { get; set; }=string.Empty;
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }=string.Empty;
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }=string.Empty;
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }=string.Empty;
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }=string.Empty;
        [JsonPropertyName("pfNumber")]
        public string PFNumber { get; set; }=string.Empty;
        [JsonPropertyName("solId")]
        public string SolId { get; set; }=string.Empty;
        [JsonPropertyName("roleId")]
        public long RoleId { get; set; }
        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }=string.Empty;
        [JsonPropertyName("roleGroupId")]
        public long RoleGroupId { get; set; }
        [JsonPropertyName("roleGroup")]
        public string RoleGroup { get; set; }=string.Empty;
        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }
        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }=string.Empty;
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }
        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }
        [JsonPropertyName("isLogged")]
        public bool IsLogged { get; set; }
        [JsonPropertyName("view")]
        public object View { get; set; }
        [JsonPropertyName("requiresPasswordChange")]
        public bool RequiresPasswordChange { get; set; }
        [JsonPropertyName("lastLoginIpAddress")]
        public string LastLoginIpAddress { get; set; }=string.Empty;
    }
}
