namespace Grc.Middleware.Api.Http.Responses {
    public class UsernameValidationResponse {
        public bool IsValid { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public long UserId { get; set; }
    }
}
