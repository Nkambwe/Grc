namespace Grc.ui.App.Http.Requests {
    public class GrcInsertRequest<T>: GrcRequest {
        public T Record {get;set;}
    }

}
