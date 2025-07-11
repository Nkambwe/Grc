namespace Grc.Middleware.Api.Utils {

    public interface IServiceLogger {
        string Id { set; get; }
        string Channel { set; get; }
        void LogActivity(string message, string type = "MSG");
    }

}
