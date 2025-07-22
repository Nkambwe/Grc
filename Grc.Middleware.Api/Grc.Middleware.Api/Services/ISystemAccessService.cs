using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Services {
    public interface ISystemAccessService: IBaseService {
        Task<SystemUser> GetByIdAsync(long id);
        Task<SystemUser> GetByEmailAsync(string email);
        Task<SystemUser> GetByUsernameAsync(string username);
        Task<string> GetUserRoleAsync(long userId);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
    }
}
