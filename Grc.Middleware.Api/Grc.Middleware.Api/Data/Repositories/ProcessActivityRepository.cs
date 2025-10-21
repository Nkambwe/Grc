using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ProcessActivityRepository : Repository<ProcessActivity>, IProcessActivityRepository
    {
        public ProcessActivityRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }


    }
}


