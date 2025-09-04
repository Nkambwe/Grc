using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {

    public class DepartmentRepository : Repository<Department>, IDepartmentRepository {

        public DepartmentRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {
        }

    }

}
