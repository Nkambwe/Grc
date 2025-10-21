using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class AuditTaskRepository : Repository<AuditTask>, IAuditTaskRepository
    {
        public AuditTaskRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }

    }
}


