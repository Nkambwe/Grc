using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {

    public class BaseService : IBaseService {

        protected readonly IServiceLogger Logger;
        protected readonly IUnitOfWorkFactory UowFactory;
        public BaseService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory) { 
            Logger = loggerFactory.CreateLogger();
            UowFactory = uowFactory;
        }
    }

}
