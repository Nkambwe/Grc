using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class GuideDocumentTypeRepository : Repository<RegulatoryDocumentType>, IRegulatoryDocumentTypeRepository
    {
        public GuideDocumentTypeRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }
}


