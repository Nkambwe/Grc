namespace Grc.Middleware.Api.Http.Responses {
    public class SecuritySettings {
        public bool ExpirePassword { get; set; }
        public int ExipreyPeriod { get; set; }
        public bool CanUseOldPassword { get; set; }
        public bool AllowAdmininsToResetPasswords { get; set; }
    }
}
