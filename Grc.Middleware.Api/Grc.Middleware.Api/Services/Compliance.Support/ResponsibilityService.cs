using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services.Compliance.Support {
    public class ResponsibilityService : BaseService, IResponsibilityService
    {
        public ResponsibilityService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }
    }
}
