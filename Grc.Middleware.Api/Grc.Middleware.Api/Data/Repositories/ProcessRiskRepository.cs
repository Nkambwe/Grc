using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ProcessRiskRepository : Repository<ProcessRisk>, IProcessRiskRepository {
        public ProcessRiskRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) 
            : base(loggerFactory, _context) {
        }
    }
}
