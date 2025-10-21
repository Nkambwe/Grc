using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class AuthenticationResponse {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("pfNumber")]
        public string PFNumber { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        public string Password { get; set; }

        [JsonPropertyName("solId")]
        public string SolId { get; set; }

        [JsonPropertyName("roleId")]
        public long RoleId { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }

        [JsonPropertyName("roleGroup")]
        public string RoleGroup { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }
        
        [JsonPropertyName("departmentCode")]
        public string DepartmentCode { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }

        [JsonPropertyName("active")]
        public bool IsActive { get; set; }
        
        [JsonPropertyName("deleted")]
        public bool IsDeleted { get; set; }
        
        [JsonPropertyName("verified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("checkVerified")]
        public bool CheckVerified { get; set; }

        [JsonPropertyName("approved")]
        public bool IsApproved { get; set; }
        
        [JsonPropertyName("checkApproval")]
        public bool CheckApproval { get; set; }

        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonPropertyName("isAdmin")]
        public bool IsAdministrator { get; set; }
       
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("url")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("favourites")]
        public List<string> Favourites { get; set; }

        [JsonPropertyName("views")]
        public List<string> Views { get; set; }

        [JsonPropertyName("claims")]
        public Dictionary<string, object> Claims { get; set; } = new Dictionary<string, object>();
    }
}
