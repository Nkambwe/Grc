using Grc.ui.App.Models;
using Microsoft.AspNetCore.Html;

namespace Grc.ui.App.Factories {
    public class PageModelFactory: IPageModelFactory {
        public async Task<PopupModel> PreparePoupModelAsync(string pageTitle, string componentName, int pageLavel, IHtmlContent pageContent){ 
            return await Task.FromResult(new PopupModel(){ 
                PageTitle=pageTitle, 
                ComponentName=componentName, 
                PageLavel=pageLavel, 
                PageContent=pageContent
                
            });
        }
    }
}
