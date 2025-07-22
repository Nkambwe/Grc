namespace Grc.ui.App.Utils {

    public class MiddlewareOptions {
         public const string SectionName = "MiddlewareOptions";
         public string BaseUrl { get; set; } = string.Empty;
         public string ProdBaseUrl { get; set; } = string.Empty;
         public int AppSessionTime { get; set; }
         public int IdleSessionTime { get; set; }
     }

}
