using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Grc.Middleware.Api.Data {

    public class GrcContextFactory: IDesignTimeDbContextFactory<GrcContext> {

        private readonly IServiceLogger _logger;
        private readonly IEnvironmentProvider _environment;
        private readonly IDataConnectionProvider _dataConnectionProvider;

        public GrcContextFactory(IServiceLoggerFactory loggerFactory, 
                                 IDataConnectionProvider dataConnectionProvider,
                                 IEnvironmentProvider environment) { 
            _logger = loggerFactory.CreateLogger("grc_middleware_log");
            _logger.Channel = $"DBCONNECTION-{DateTime.Now:yyyyMMddHHmmss}";
            _dataConnectionProvider = dataConnectionProvider;
            _environment = environment;
        }

        public GrcContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<GrcContext>();
            try {
                var isLive = _environment.IsLive;
                var connectionVar = _dataConnectionProvider.DefaultConnection;

                if(!string.IsNullOrWhiteSpace(connectionVar)){ 
                    //Retrieve the connection string from environment variables
                    string connectionString = Environment.GetEnvironmentVariable(connectionVar);
                    if (!string.IsNullOrEmpty(connectionString)) {
                        string decryptedString = HashGenerator.DecryptString(connectionString);
                        if(isLive){ 
                            _logger.LogActivity($"CONNECTION URL :: {connectionString}", "INFO");
                        } else {
                            _logger.LogActivity($"CONNECTION URL :: {decryptedString}", "INFO");
                        }

                        optionsBuilder.UseSqlServer(decryptedString);
                    } else {
                        string msg="Environmental variable name 'GRC_DBCONNECTION_ENV' which holds connection string not found";
                        _logger.LogActivity(msg, "DB_ERROR");
                        throw new Exception(msg);
                    }
                } else {

                }
                
            } catch (Exception e) {
                _logger.LogActivity($"Database connection failed. {e.Message}", "ERROR");
            }

            return new GrcContext(optionsBuilder.Options);
        }
    }
}
