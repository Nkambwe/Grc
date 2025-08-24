using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class BranchRepository : Repository<Branch>, IBranchRepository {

        public BranchRepository(IServiceLoggerFactory loggerFactory, GrcContext context) 
            : base(loggerFactory, context) {
        }

        public async Task<Branch> GetByIdAsync(long id)
            => await GetAsync(id, true);

        public async Task<Branch> GetBySoleIDAsync(string solId)
            => await GetAsync(b => b.SolId == solId, true);

        public async Task<IList<Branch>> GetBranchesAsync()
            => await GetAllAsync(includeDeleted: false);
    }
}
