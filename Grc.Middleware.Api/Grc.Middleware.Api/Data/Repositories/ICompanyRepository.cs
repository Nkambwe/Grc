using Grc.Middleware.Api.Data.Entities;

namespace Grc.Middleware.Api.Data.Repositories {
   public interface ICompanyRepository : IRepository<Company> {
        Task<Company> GetByNameAsync(string name);
        Task<IList<Company>> GetActiveCompaniesAsync();
   }
}
