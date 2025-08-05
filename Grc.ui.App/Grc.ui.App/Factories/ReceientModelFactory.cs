using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class ReceientModelFactory : IReceientModelFactory {
        public ILocalizationService ILocalize {get; }

        public IEnumerable<RecentModel> RecentItems { get;set;}

        public ReceientModelFactory(ILocalizationService _iLocalize) {
            ILocalize = _iLocalize;
        }
    }

}
