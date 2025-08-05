using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {
    public interface IReceientModelFactory{ 
        ILocalizationService ILocalize {get;}
        IEnumerable<RecentModel> RecentItems {get; set;}
    }
}
