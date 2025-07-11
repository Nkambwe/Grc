using Microsoft.Extensions.Options;

namespace Grc.Middleware.Api.Utils {
    public class DataConnectionProvider : IDataConnectionProvider {

        private readonly DataConnectionOptions _options;
        public string DefaultConnection => _options.DefaultConnection;

        public DataConnectionProvider(IOptions<DataConnectionOptions> options) {
            _options = options.Value;
        }

    }
}
