using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class AuditExceptionRepository : Repository<AuditException>, IAuditExceptionRepository
    {
        public AuditExceptionRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }
    }
}


