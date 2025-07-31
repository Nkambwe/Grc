namespace Grc.Middleware.Api.Http.Requests {

    public class LoginRequest {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsPersistent { get; set; }
        public bool IsValidated { get; set; }
        public string Action { get; set; }
        public string IPAddress { get; set; }
        public string[] EncryptFields { get; set; }
        public string[] DecryptFields { get; set; }
    }
}
