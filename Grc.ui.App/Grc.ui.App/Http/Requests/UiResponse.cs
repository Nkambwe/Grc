using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class UiResponse {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("redirectUrl")]
        public string RedirectUrl { get; set; }
        [JsonPropertyName("data")]
        public object Data { get; set; }        

        public static UiResponse Ok(string message = "", object data = null) => new() { Success = true, Message = message, Data = data };

        public static UiResponse Fail(string message, string redirectUrl = null) => new() { Success = false, Message = message, RedirectUrl = redirectUrl };
    }
}
