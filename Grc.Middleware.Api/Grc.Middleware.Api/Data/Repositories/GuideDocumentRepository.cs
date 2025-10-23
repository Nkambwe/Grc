using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class GuideDocumentRepository : Repository<RegulatoryDocument>, IRegulatoryDocumentRepository
    {
        public GuideDocumentRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }
}


