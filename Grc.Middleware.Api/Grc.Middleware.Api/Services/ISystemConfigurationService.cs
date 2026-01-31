using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {

    public interface ISystemConfigurationService : IBaseService {

        Task<bool> ExistsAsync(Expression<Func<SystemConfiguration, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);
        Task<ConfigurationParameterResponse<T>> GetConfigurationAsync<T>(string paramName);
        Task<ConfigurationResponse> GetAllConfigurationAsync();
        Task<bool> UpdateConfigurationAsync(SystemConfigurationRequest request, string username);
        Task<IList<SystemConfiguration>> GetAllAsync(Expression<Func<SystemConfiguration, bool>> predicate, bool includeDeleted);
        Task<PagedResult<SystemConfiguration>> PagedUsersAsync(CancellationToken token, int page, int size, Expression<Func<SystemConfiguration, bool>> predicate = null, bool includeDeleted = false);
    }

}
