using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Services {
    public interface ICompanyService : IBaseService {
        Task<bool> CreateCompanyAsync(Company company);
    }
}