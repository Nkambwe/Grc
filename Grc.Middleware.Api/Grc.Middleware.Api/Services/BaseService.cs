using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {

    public class BaseService : IBaseService {
        
        public IServiceLogger Logger {get;}

        public IMapper Mapper {get;}

        public IUnitOfWorkFactory UowFactory  {get;}
        public BaseService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) { 
            Logger = loggerFactory.CreateLogger();
            UowFactory = uowFactory;
            Mapper = mapper;
        }

    }

}
