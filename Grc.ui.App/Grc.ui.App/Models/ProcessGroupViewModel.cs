using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ProcessGroupViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("groupDescription")]
        public string GroupDescription { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }

}
