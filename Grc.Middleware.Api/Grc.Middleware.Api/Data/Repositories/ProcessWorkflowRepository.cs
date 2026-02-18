using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ProcessWorkflowRepository : Repository<ProcessWorkflow>, IProcessWorkflowRepository {
        public ProcessWorkflowRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }
    }
}
