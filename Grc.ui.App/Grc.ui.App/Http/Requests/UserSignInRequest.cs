namespace Grc.ui.App.Http.Requests {

    public class UserSignInRequest : GrcRequest {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsValidated { get; set; } = false;
        public bool IsPersistent { get; set; }
    }


}
