using Microsoft.AspNetCore.Html;

namespace Grc.ui.App.Models {

    public class PopupModel {
        public string PageTitle{ get;set;}
        public string ComponentName{ get;set;} 
        public int PageLavel{ get;set;} 
        public IHtmlContent PageContent{ get;set;} 
    }
}
