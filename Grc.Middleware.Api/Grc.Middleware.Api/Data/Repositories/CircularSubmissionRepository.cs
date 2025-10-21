using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class CircularSubmissionRepository : Repository<CircularSubmission>, ICircularSubmissionRepository
    {
        public CircularSubmissionRepository(IServiceLoggerFactory loggerFactory, GrcContext _context) : base(loggerFactory, _context)
        {
        }

    }
}


