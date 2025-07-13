
using System.Text.Json.Serialization;

namespace Grc.ui.App.Dtos {

    public class NotificationMessage {

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        /// <summary>
        /// Provider can be SweatAlert or toastr
        /// </summary>
        [JsonPropertyName("provider")]
        public string Provider { get; set; }

    }

}
