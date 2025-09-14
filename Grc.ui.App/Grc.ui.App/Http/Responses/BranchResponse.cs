using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class BranchResponse {
        /// <summary>
        /// Gets or sets branch Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets company Id
        /// </summary>
        [JsonPropertyName("companyId")]
        public long CompanyId { get; set; }
        /// <summary>
        /// Gets or sets branch name
        /// </summary>
        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }
        /// <summary>
        /// Gets or sets company name
        /// </summary>
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }
        /// <summary>
        /// Gets or sets branch sol ID
        /// </summary>
        [JsonPropertyName("solId")]
        public string SolId { get; set; }
        /// <summary>
        /// Get or set the record as deleted
        /// </summary>
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get;set; }
        /// <summary>
        /// Get or Set Creation date
        /// </summary>
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Get or Set Person who created record
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Get or Set Modification Date
        /// </summary>
        [JsonPropertyName("modifiedOn")]
        public DateTime? LastModifiedOn {get;set;}
        /// <summary>
        /// Get or Set Person who modified record
        /// </summary>
        [JsonPropertyName("modifiedBy")]
        public string LastModifiedBy { get;set; }
    }
}
