using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ProcessGroupRepository : Repository<ProcessGroup>, IProcessGroupRepository
    {
        public ProcessGroupRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }
    }
}


