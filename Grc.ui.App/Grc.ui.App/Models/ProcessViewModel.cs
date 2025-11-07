using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class ProcessViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("currentVersion")]
        public string CurrentVersion { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("processStatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("onFile")]
        public bool OnFile { get; set; }

        [JsonPropertyName("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("approvalStatus")]
        public string ApprovalStatus { get; set; }

        [JsonPropertyName("approvalComment")]
        public string ApprovalComment { get; set; }

        [JsonPropertyName("onholdReason")]
        public string OnholdReason{ get; set; }

        [JsonPropertyName("unitId")]
        public long UnitId { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }
    }

}
