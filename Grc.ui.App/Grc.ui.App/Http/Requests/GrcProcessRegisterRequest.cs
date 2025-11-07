using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class GrcProcessRegisterRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isNew")]
        public bool IsNew => Id == 0;

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("currentVersion")]
        public string CurrentVersion { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [JsonPropertyName("originalOnFile")]
        public bool OriginalOnFile { get; set; }

        [JsonPropertyName("processStatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("approvalStatus")]
        public string ApprovalStatus { get; set; }

        [JsonPropertyName("approvalComment")]
        public string ApprovalComment { get; set; }

        [JsonPropertyName("onholdReason")]
        public string OnholdReason { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }

        [JsonPropertyName("unitId")]
        public long UnitId { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("responsibilityId")]
        public long ResponsibilityId { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}
