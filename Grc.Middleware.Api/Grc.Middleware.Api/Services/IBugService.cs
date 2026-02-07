using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {
    public interface IBugService : IBaseService {
        Task<SystemError> GetBugAsync(long recordId);
        Task<PagedResult<SystemError>> GetBugsAsync(BugListRequest request);
        Task<IList<SystemError>> GetAllAsync(Expression<Func<SystemError, bool>> where, bool includeDeleted);
        Task<PagedResult<SystemError>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<SystemError, bool>> predicate);
        Task<bool> InsertSystemErrorAsync(BugRequest request, string username);
        Task<bool> UpdateStatusAsync(long recordId, string status, string username);
        Task<bool> UpdateErrorAsync(BugRequest request, string username);
        Task<bool> ExistsAsync(Expression<Func<SystemError, bool>> predicate);
    }
}
