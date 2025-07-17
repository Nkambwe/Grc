using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Repositories {
    public interface ICompanyRepository : IRepository<Company> {
        Task<Company> GetByNameAsync(string name);
        Task<IList<Company>> GetActiveCompaniesAsync();
   }
}
