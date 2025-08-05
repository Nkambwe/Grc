using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {
    public interface IPinnedModelFactory { 
        ILocalizationService ILocalize {get;}
        IEnumerable<PinnedModel> PinnedItems {get; set;}
    }
}
