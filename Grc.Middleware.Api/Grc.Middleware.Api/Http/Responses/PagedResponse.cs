
namespace Grc.Middleware.Api.Http.Responses {
    /// <summary>
    /// PagedResponse Class handling list responses
    /// </summary>
    /// <typeparam name="T">Success Data object type</typeparam>
    public class PagedResponse<T> where T : class{
        public IList<T> Entities { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Size);

        public PagedResponse(IList<T> data, int totalCount, int page, int size) {
            Entities = data;
            TotalCount = totalCount;
            Page = page;
            Size = size;
        }

    }
}
