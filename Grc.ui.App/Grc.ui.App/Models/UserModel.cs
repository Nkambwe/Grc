
using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class UserModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("pfNumber")]
        public string PFNumber { get; set; }

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

        [JsonPropertyName("unitCode")]
        public string UnitCode { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("isApproved")]
        public bool IsApproved { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonPropertyName("isLogged")]
        public bool IsLogged { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        /// <summary>
        /// Get Or Send Fields intended to be encrypted
        /// </summary>
        [JsonPropertyName("encrypts")]
        public string[] EncryptFields { get; set; }
        /// <summary>
        /// Get Or Set Fields intended to be decrypted
        /// </summary>
        [JsonPropertyName("decrypts")]
        public string[] DecryptFields { get; set; }

        [JsonPropertyName("passwordChange")]
        public bool RequiresPasswordChange { get; set; }

        [JsonPropertyName("lockedDate")]
        public DateTime? LockedDate { get; set; }

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
