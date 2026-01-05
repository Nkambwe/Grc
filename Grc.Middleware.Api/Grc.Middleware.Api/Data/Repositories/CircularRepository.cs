using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class CircularRepository : Repository<Circular>, ICircularRepository {
        public CircularRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }

}


