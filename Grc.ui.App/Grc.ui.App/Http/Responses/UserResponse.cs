using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class UserResponse {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("pfNumber")]
        public string PFNumber { get; set; }

        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("solId")]
        public string SolId { get; set; }

        [JsonPropertyName("roleId")]
        public long RoleId { get; set; }

        [JsonPropertyName("roleCode")]
        public long RoleCode { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("departmentCode")]
        public long DepartmentCode { get; set; }

        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isLogged")]
        public bool IsLogged { get; set; }

        [JsonPropertyName("favourites")]
        public List<string> Favourites { get; set; }

        [JsonPropertyName("views")]
        public List<string> Views { get; set; }
    }
}
