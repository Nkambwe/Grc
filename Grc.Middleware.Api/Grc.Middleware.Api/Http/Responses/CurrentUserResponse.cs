namespace Grc.Middleware.Api.Http.Responses {
    public class CurrentUserResponse {
        public long UserId { get; set; }
        public string PersonnelFileNumber { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
