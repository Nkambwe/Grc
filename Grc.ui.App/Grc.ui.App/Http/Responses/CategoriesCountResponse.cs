namespace Grc.ui.App.Http.Responses {
    public class CategoriesCountResponse {
        public int Unclassified { get; set; }
        public int UpToDate { get; set; }
        public int Unchanged { get; set; }
        public int Proposed { get; set; }
        public int Due { get; set; }
        public int Dormant { get; set; }
        public int Cancelled { get; set; }
        public int Total { get; set; }
    }
}
