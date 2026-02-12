namespace Grc.Middleware.Api.Http.Responses {
    public class SecuritySettings {
        public bool ExpirePassword { get; set; }
        public int ExipreyPeriod { get; set; }
        public bool CanUseOldPassword { get; set; }
        public bool AllowAdmininsToResetPasswords { get; set; }
        public int MinimumPasswordLength { get; set; }
        public bool IncludeUpperCharacters { get; set; }
        public bool IncludeLowerCharacters { get; set; }
        public bool IncludeSpecialCharacters { get; set; }
        public bool AllowPasswordReuse { get; set; }
        public bool IncludeNumericCharacters { get; set; }
    }
}
