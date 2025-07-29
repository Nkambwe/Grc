using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {
    public interface IBaseService {
       IServiceLogger Logger{get;}
       IMapper Mapper{get;}
       IUnitOfWorkFactory UowFactory {get;}
    }
}
