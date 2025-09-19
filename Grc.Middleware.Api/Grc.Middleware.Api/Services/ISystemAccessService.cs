using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Responses;

namespace Grc.Middleware.Api.Services {
    public interface ISystemAccessService: IBaseService {
        /// <summary>
        /// Get user record by ID
        /// </summary>
        /// <param name="id">User ID to look for</param>
        /// <returns>
        /// Task containg System user record
        /// </returns>
        Task<SystemUser> GetByIdAsync(long id);
        /// <summary>
        /// Get user record by Email address
        /// </summary>
        /// <param name="email">User email to look for</param>
        /// <returns>
        /// Task containg System user record
        /// </returns>
        Task<SystemUser> GetByEmailAsync(string email);
        /// <summary>
        /// Get user record by username
        /// </summary>
        /// <param name="username">User role ID</param>
        /// <returns>
        /// Task containg System role name
        /// </returns>
        Task<SystemUser> GetByUsernameAsync(string username);
        /// <summary>
        /// Get user role by username address
        /// </summary>
        /// <param name="userId">User username to look for</param>
        /// <returns>
        /// Task containg System user record
        /// </returns>
        Task<string> GetUserRoleAsync(long userId);
        /// <summary>
        /// Get total count of users in the system
        /// </summary>
        /// <returns>
        /// Task containg System user count
        /// </returns>
        Task<int> GetTotalUsersCountAsync();
        /// <summary>
        /// Get dashbaord statistics
        /// </summary>
        /// <returns>
        /// Task containg count for various dasboard statistics
        /// </returns>
        Task<AdminCountResponse> GetAdminiDashboardStatisticsAsync();
        /// <summary>
        /// Get total count of active users in the system
        /// </summary>
        /// <returns>
        /// Task containg count of active system users
        /// </returns>
        Task<int> GetActiveUsersCountAsync();
        /// <summary>
        /// Validate username in the system
        /// </summary>
        /// <param name="username">Username to verify</param>
        /// <returns>
        /// Task containg verification response of username
        /// </returns>
        Task<UsernameValidationResponse> ValidateUsernameAsync(string username);
        /// <summary>
        /// Authenticate system user
        /// </summary>
        /// <param name="username">Username to authenticate</param>
        /// <returns></returns>
        Task<AuthenticationResponse> AuthenticateUserAsync(string username);
        /// <summary>
        ///  Update system user loggout
        /// </summary>
        /// <param name="userId">User ID to login</param>
        /// <param name="loginTime">Login date and time</param>
        /// <returns></returns>
        Task UpdateLastLoginAsync(long userId, DateTime loginTime);
        /// <summary>
        /// Update system user loggout
        /// </summary>
        /// <param name="userId">User ID to login</param>
        /// <param name="loginTime">Login date and time</param>
        /// <returns></returns>
        Task<bool> UpdateLoginStatusAsync(long userId, DateTime loginTime);
        /// <summary>
        /// Log failed login for the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="ipAddress">IP Address</param>
        /// <returns></returns>
        Task<bool> LogFailedLoginAsync(long userId, string ipAddress);
        /// <summary>
        /// Lock user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns></returns>
        Task LockUserAccountAsync(long userId);
        /// <summary>
        /// Process user workspace
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="ipAddress">IP Address</param>
        /// <returns>Task containing user workspace</returns>
        Task<WorkspaceResponse> GetWorkspaceAsync(long userId, string ipAddress);

        /// <summary>
        /// Get role record by ID
        /// </summary>
        /// <param name="id">Role ID to look for</param>
        /// <returns>
        /// Task containg System role record
        /// </returns>
        Task<SystemRole> GetRoleByIdAsync(long id);

        /// <summary>
        /// Get role Group record by ID
        /// </summary>
        /// <param name="id">Group ID to look for</param>
        /// <returns>
        /// Task containg System role group record
        /// </returns>
        Task<SystemRoleGroup> GetRoleGroupByIdAsync(long id);
    }
}
