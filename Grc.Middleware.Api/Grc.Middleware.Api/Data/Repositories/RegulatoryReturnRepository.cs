using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class RegulatoryReturnRepository : Repository<RegulatoryReturn>, IRegulatoryReturnRepository
    {
        public RegulatoryReturnRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }


}


