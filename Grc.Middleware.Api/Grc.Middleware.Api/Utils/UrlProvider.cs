using Microsoft.Extensions.Options;

namespace Grc.Middleware.Api.Utils { 
    public class UrlProvider : IUrlProvider {
        private readonly UrlOptions _options;

        public string BaseUrl => _options.BaseUrl;

        public UrlProvider(IOptions<UrlOptions> options) {
            _options = options.Value;
        }
    }

}
