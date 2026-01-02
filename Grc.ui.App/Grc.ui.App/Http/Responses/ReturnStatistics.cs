namespace Grc.ui.App.Http.Responses {
    public class ReturnStatistics {
        public string Authority { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public int Open { get; set; }
        public int Closed { get; set; }
        public int Breach { get; set; }
        public int Total { get; set; }
    }

}
