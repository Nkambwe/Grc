using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class DepartmentUnitRepository : Repository<DepartmentUnit>, IDepartmentUnitRepository {

        public DepartmentUnitRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }

    }

}
