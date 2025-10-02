using Grc.Middleware.Api.Data.Entities.Compliance;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class GuideDocumentTypeRepository : Repository<GuideDocumentType>, IGuideDocumentTypeRepository
    {
        public GuideDocumentTypeRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }
}


