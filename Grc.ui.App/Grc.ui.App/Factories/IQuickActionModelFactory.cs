using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {
    public interface  IQuickActionModelFactory {
        ILocalizationService ILocalize {get;}
        public IEnumerable<QuickActionModel> QuickActionItems {get; set;}
    }
}
