namespace Grc.ui.App.Http.Responses {

    public class PagedResponse<T> {
        public List<T> Entities { get; set; } = new();
        public int TotalCount  { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages { get; set; }

    }

}
