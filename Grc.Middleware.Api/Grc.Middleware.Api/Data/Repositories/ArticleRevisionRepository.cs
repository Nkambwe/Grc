using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class ArticleRevisionRepository : Repository<ArticleRevision>, IArticleRevisionRepository {
        public ArticleRevisionRepository(IServiceLoggerFactory loggerFactory, GrcContext context) 
            : base(loggerFactory, context) {
        }
    }
}


