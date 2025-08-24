namespace Grc.Middleware.Api.Http.Responses {

    public class AdminCountResponse {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeactivatedUsers { get; set; }
        public int UnApprovedUsers { get; set; }
        public int UnverifiedUsers { get; set; }
        public int DeletedUsers { get; set; }
        public int TotalBugs { get; set; }
        public int NewBugs { get; set; }
        public int BugFixes { get; set; }
        public int BugProgress { get; set; }
        public int UserReportedBugs { get; set; }
    }

}
