using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class AuditReportRepository : Repository<AuditReport>, IAuditReportRepository
    {
        public AuditReportRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }
    }
}


