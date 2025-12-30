using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Data.Repositories {

    public class StatutoryArticleRepository : Repository<StatutoryArticle>, IStatutoryArticleRepository {
        public StatutoryArticleRepository(IServiceLoggerFactory loggerFactory, GrcContext context)
            : base(loggerFactory, context)
        {
        }

    }
}


