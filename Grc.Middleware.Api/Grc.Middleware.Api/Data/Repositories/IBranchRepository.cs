using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Repositories {

    public interface IBranchRepository: IRepository<Branch> {
        Task<Branch> GetByIdAsync(long id);
        Task<Branch> GetBySoleIDAsync(string solId);
        Task<IList<Branch>> GetBranchesAsync();
    }
}
