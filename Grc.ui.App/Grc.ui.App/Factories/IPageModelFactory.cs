using Grc.ui.App.Models;
using Microsoft.AspNetCore.Html;

namespace Grc.ui.App.Factories {

    public interface IPageModelFactory {
        Task<PopupModel> PreparePoupModelAsync(string pageTitle, string componentName, int pageLavel, IHtmlContent pageContent);
    }
}
