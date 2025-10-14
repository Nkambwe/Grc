using Grc.Middleware.Api.Data.Entities.Compliance;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class RegulatoryTypeRepository : Repository<RegulatoryType>, IRegulatoryTypeRepository
    {
        public RegulatoryTypeRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }


}


