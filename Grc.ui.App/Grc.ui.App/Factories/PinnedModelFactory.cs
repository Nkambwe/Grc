using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {
    public class PinnedModelFactory : IPinnedModelFactory {
        public ILocalizationService ILocalize {get; }

        public IEnumerable<PinnedModel> PinnedItems { get;set;}

        public PinnedModelFactory(ILocalizationService _iLocalize) {
            ILocalize = _iLocalize;
        }
    }
}
