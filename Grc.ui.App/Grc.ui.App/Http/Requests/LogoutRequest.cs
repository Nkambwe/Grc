using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class LogoutRequest : GrcRequest { 
        [JsonPropertyName("isLoggedOut")]
        public bool IsLoggedOut { get; set; }
    }

}
