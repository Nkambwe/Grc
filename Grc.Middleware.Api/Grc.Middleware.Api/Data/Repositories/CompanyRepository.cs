using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Data.Repositories {
    public class CompanyRepository : Repository<Company>, ICompanyRepository {

        public CompanyRepository(IServiceLoggerFactory loggerFactory, GrcContext context) 
            : base(loggerFactory, context) {
        }

        public async Task<Company> GetByNameAsync(string name)
            => await GetAsync(c => c.CompanyName == name);

        public async Task<IList<Company>> GetActiveCompaniesAsync()
            => await GetAllAsync(includeDeleted: false);
    }
}
