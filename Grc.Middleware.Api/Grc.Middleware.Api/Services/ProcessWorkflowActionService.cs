using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {
    public class ProcessWorkflowActionService : BaseService, IProcessWorkflowActionService {
        public ProcessWorkflowActionService(
            IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, 
            IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }
    }
}
