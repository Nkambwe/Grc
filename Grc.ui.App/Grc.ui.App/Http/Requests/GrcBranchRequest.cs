using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcBranchRequest {
        /// <summary>
        /// Get or Set ID for the unit
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Get or Set ID of user sending request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }
        /// <summary>
        /// Get or Set branch SolId
        /// </summary>
        [JsonPropertyName("solId")]
        public string SolId { get; set; }
        /// <summary>
        /// Get or Set branch name
        /// </summary>
        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }
        /// <summary>
        /// Get or Set ID of company
        /// </summary>
        [JsonPropertyName("companyId")]
        public long CompanyId { get; set; }
        /// <summary>
        /// Get or Set whether branch is deleted
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
