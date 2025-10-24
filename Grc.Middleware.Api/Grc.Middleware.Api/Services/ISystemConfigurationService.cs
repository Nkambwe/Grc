using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services
{
    public interface ISystemConfigurationService : IBaseService
    {
        Task<SystemConfiguration> GetConfigurationAsync(string paramName);
        Task<bool> UpdateConfigurationAsync(SystemConfigurationRequest request);
        Task<IList<SystemConfiguration>> GetAllAsync(Expression<Func<SystemConfiguration, bool>> predicate, bool includeDeleted);
        Task<PagedResult<SystemConfiguration>> PagedUsersAsync(CancellationToken token, int page, int size, Expression<Func<SystemConfiguration, bool>> predicate = null, bool includeDeleted = false);
    }
}
