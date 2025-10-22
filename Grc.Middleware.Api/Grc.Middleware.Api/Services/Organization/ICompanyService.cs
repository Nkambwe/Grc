using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Services.Organization {
    public interface ICompanyService : IBaseService {
        Task<bool> CreateCompanyAsync(Company company);
        Task<Company> GetDefaultCompanyAsync();
    }
}