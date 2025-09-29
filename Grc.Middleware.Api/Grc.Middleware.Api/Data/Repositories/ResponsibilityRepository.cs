using Grc.Middleware.Api.Data.Entities.Compliance;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ResponsibilityRepository : Repository<Responsibility>, IResponsibilityRepository
    {
        public ResponsibilityRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }


}


