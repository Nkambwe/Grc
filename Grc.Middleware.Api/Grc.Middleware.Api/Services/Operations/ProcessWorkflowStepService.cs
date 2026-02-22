using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services.Operations {
    public class ProcessWorkflowStepService : BaseService, IProcessWorkflowStepService {
        public ProcessWorkflowStepService(
            IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }
    }
}
