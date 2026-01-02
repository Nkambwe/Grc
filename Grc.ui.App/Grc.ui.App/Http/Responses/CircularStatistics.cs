namespace Grc.ui.App.Http.Responses {
    public class CircularStatistics {
        public string Period { get; set; } = string.Empty;  
        public string Authority { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
        public int Breach { get; set; }
    }

}
