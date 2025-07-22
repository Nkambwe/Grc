namespace Grc.ui.App.Http.Requests {

    public class UserSignInRequest : GrcRequest {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
        public bool IsPersistent { get; set; }
    }

    
}
