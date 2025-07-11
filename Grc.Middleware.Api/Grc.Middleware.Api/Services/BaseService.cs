using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {

    public class BaseService : IBaseService {

        protected readonly IServiceLogger Logger;

        public BaseService(IServiceLoggerFactory loggerFactory) { 
             Logger = loggerFactory.CreateLogger();
        }
    }

}
