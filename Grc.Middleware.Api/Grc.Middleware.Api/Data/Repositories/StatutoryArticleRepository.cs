using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class StatutoryArticleRepository : Repository<StatutoryArticle>, IStatutoryArticleRepository
    {
        public StatutoryArticleRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context)
        {
        }
    }


}


