using Microsoft.Extensions.Options;

namespace Grc.ui.App.Utils {
    public class EnvironmentProvider : IEnvironmentProvider {

        private readonly EnvironmentOptions _options;
        public bool IsLive => _options.IsLive;

        public EnvironmentProvider(IOptions<EnvironmentOptions> options) {
            _options = options.Value;
        }
    }
}
