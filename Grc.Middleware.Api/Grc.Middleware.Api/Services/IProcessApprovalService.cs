using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services
{

    public interface IProcessApprovalService
    {
        int Count();
        int Count(Expression<Func<OperationProcess, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<ProcessApproval, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<ProcessApproval> GetAsync(long id, bool includeDeleted = false);
        Task<ProcessApproval> GetAsync(Expression<Func<ProcessApproval, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessApproval, object>>[] includes);
        Task<IList<ProcessApproval>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessApproval, object>>[] includes);
        Task<bool> InsertAsync(ProcessApprovalRequest request);
        Task<bool> UpdateAsync(ApprovalRequest request, bool includeDeleted = false);
        bool Delete(IdRequest request);
        Task<bool> DeleteAsync(IdRequest request);
        Task<PagedResult<ProcessApproval>> PageProcessApprovalStatusAsync(int pageIndex, int pageSize, bool includeDeleted, params Expression<Func<ProcessApproval, object>>[] includes);
       
    }
}
