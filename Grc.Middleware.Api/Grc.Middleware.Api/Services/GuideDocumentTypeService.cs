using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services
{
    public class GuideDocumentTypeService : BaseService, IGuideDocumentTypeService
    {
        public GuideDocumentTypeService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper)
        {
        }
    }
}
