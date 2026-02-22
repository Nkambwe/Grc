using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class ProcessDeleteViewModel {
        
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }

}
