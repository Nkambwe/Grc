using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {

    public class RegistrationRequest: MiddlewareRequest {
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("shortName")]
        public string Alias { get; set; }

        [JsonPropertyName("regNumber")]
        public string RegNumber { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

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

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isLogged")]
        public bool IsLogged { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }
    }
}
