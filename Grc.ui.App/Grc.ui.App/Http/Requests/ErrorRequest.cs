namespace Grc.ui.App.Http.Requests {
    public class ErrorRequest {
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string Severity { get; set; }
        public string StackTrace { get; set; }
    }
}
