using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ProcessTagViewModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("tagName")]
        public string TagName { get; set; }

        [JsonPropertyName("tagDescription")]
        public string TagDescription { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }

}
