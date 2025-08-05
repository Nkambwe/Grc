using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class QuickActionModelFactory : IQuickActionModelFactory {
        public ILocalizationService ILocalize { get;}

        public IEnumerable<QuickActionModel> QuickActionItems { get; set; }
        public QuickActionModelFactory(ILocalizationService _iLocalize) {
            ILocalize = _iLocalize;
        }
    }
}
