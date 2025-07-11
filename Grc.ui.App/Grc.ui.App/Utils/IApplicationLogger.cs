namespace Grc.ui.App.Utils {

    public interface IApplicationLogger {
        string Id { set; get; }
        string Channel { set; get; }
        void LogActivity(string message, string type = "MSG");
    }

}
