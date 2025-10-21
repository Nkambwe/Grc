using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ProcessTagRepository : Repository<ProcessTag>, IProcessTagRepository
    {
        public ProcessTagRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }
    }
}


