using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface ISystemAccessService : IGrcBaseService {

        #region Dashboard
        /// <summary>
        /// Get total count for all users in the system
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="requestingUserId">User ID of the requesting user</param>
        /// <returns>Task containing number of users in the system</returns>
        Task<RecordCountResponse> CountAllUsersAsync(long requestingUserId, string ipAddress);

        /// <summary>
        /// Get all user statistics
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="requestingUserId">User ID of the requesting user</param>
        /// <returns>Task containing user dashboard statistics/returns>
        Task<AdminCountResponse> StatisticAsync(long requestingUserId, string ipAddress);

        /// <summary>
        /// Get count of active users in the system
        /// </summary>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="requestingUserId">User ID of the requesting user</param>
        /// <returns>Task containing count of active users</returns>
        Task<RecordCountResponse> CountActiveUsersAsync(long requestingUserId, string ipAddress);

        #endregion

        #region System Activities

        Task<GrcResponse<GrcSystemActivityResponse>> GetSystemActivityIdAsync(long userId, long recordId, string ipAddress);

        Task<GrcResponse<PagedResponse<GrcSystemActivityResponse>>> GetPagedActivitiesAsync(TableListRequest request);

        #endregion

        #region System Users

        /// <summary>
        /// Get user info by Database ID
        /// </summary>
        /// <param name="userId">User ID to look for</param>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="recordId">Record IF</param>
        /// <returns>
        /// Task containing user with provided ID or null
        /// </returns>
        Task<GrcResponse<UserResponse>> GetUserByIdAsync(long userId, long recordId, string ipAddress);

        /// <summary>
        /// Get user info by user email address
        /// </summary>
        /// <param name="email">User email address to look for</param>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="requestingUserId">User ID of the requesting user</param>
        /// <returns>
        /// Task containing user with provided email address or null
        /// </returns>
        Task<GrcResponse<UserResponse>> GetUserByEmailAsync(string email, long requestingUserId, string ipAddress);

        /// <summary>
        /// Get user info by user email address
        /// </summary>
        /// <param name="username">Username to look for</param>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <param name="requestingUserId">User ID of the requesting user</param>
        /// <returns>
        /// Task containing user with provided email address or null
        /// </returns>
        Task<GrcResponse<UserResponse>> GetUserByUsernameAsync(string username, long requestingUserId, string ipAddress);

        /// <summary>
        /// Get a list of users in the system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GrcResponse<ListResponse<UserResponse>>> GetUsersAsync(GrcRequest request);
        /// <summary>
        /// Get a list of pagednated user records
        /// </summary>
        /// <param name="request">Pagenated request</param>
        /// <returns></returns>
        Task<GrcResponse<PagedResponse<UserResponse>>> GetPagedUsersAsync(TableListRequest request);
        /// <summary>
        /// Delete system user from the system
        /// </summary>
        /// <param name="request">User record to delete</param>
        /// <returns>Task containing persistance status of this user record</returns>
        Task<GrcResponse<ServiceResponse>> DeleteUserAsync(GrcIdRequest request);
        /// <summary>
        /// Get user validation
        /// </summary>
        /// <param name="model">Login model to validate agnest</param>
        /// <returns>
        /// </returns>
        Task<GrcResponse<UsernameValidationResponse>> ValidateUsernameAsync(UsernameValidationModel model);
        /// <summary>
        /// Update the user logged in status
        /// </summary>
        /// <param name="userId">User ID for user to logout</param>
        /// <param name="isLoggedIn">Logged in status</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns></returns>
        Task UpdateLoggedInStatusAsync(long userId, bool isLoggedIn, string ipAddress);
        /// <summary>
        /// Create a new user in the system
        /// </summary>
        /// <param name="userRecord">User record to add</param>
        /// <param name="userId">User ID for user to logout</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns></returns>
        Task<GrcResponse<ServiceResponse>> CreateUserAsync(UserViewModel userRecord, long userId, string ipAddress);
        /// <summary>
        /// Update system user in the system
        /// </summary>
        /// <param name="userRecord">User record to update</param>
        /// <param name="userId">User ID for user to look out for</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns>Task containing update status of this user</returns>
        Task<GrcResponse<ServiceResponse>> UpdateUserAsync(UserViewModel userRecord, long userId, string ipAddress);
        #endregion

        #region Roles

        /// <summary>
        /// Get System Role info by Database ID
        /// </summary>
        /// <param name="recordId">Record ID to look for</param>
        /// <param name="userId">User ID for user initiating action</param>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <returns>Task containing system role with provided ID or null</returns>
        Task<GrcResponse<GrcRoleResponse>> GetRoleByIdAsync(long recordId, long userId, string ipAddress);
        /// <summary>
        /// Get a list of roles in the system
        /// </summary>
        /// <param name="request">Role request object</param>
        /// <returns>Task containing a list of system roles or empty list if none</returns>
        Task<GrcResponse<ListResponse<GrcRoleResponse>>> GetRolesAsync(GrcRequest request);
        /// <summary>
        /// Get a list of pagednated system roles records
        /// </summary>
        /// <param name="request">Pagenated request</param>
        /// <returns>Task containing a list of system roles or empty list if none</returns>
        Task<GrcResponse<PagedResponse<GrcRoleResponse>>> GetPagedRolesAsync(TableListRequest request);
        /// <summary>
        /// Create a new system role in the system
        /// </summary>
        /// <param name="roleRecord">System role record to add</param>
        /// <param name="userId">User ID for user to logout</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns>Task containing persistance status of this role</returns>
        Task<GrcResponse<ServiceResponse>> CreateRoleAsync(RoleViewModel roleRecord, long userId, string ipAddress);
        /// <summary>
        /// Update system role in the system
        /// </summary>
        /// <param name="roleRecord">System role record to update</param>
        /// <param name="userId">User ID for user to look out for</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns>Task containing update status of this role</returns>
        Task<GrcResponse<ServiceResponse>> UpdateRoleAsync(RoleViewModel roleRecord, long userId, string ipAddress);
        /// <summary>
        /// Delete system role from the system
        /// </summary>
        /// <param name="request">System record to delete</param>
        /// <returns>Task containing persistance status of this user role</returns>
        Task<GrcResponse<ServiceResponse>> DeleteRoleAsync(GrcIdRequest request);

        #endregion

        #region Role Groups

        /// <summary>
        /// Get System Role Group info by Database ID
        /// </summary>
        /// <param name="recordId">Record ID to look for</param>
        /// <param name="userId">User ID for user initiating action</param>
        /// <param name="ipAddress">Current login IP Address of the requesting user</param>
        /// <returns>Task containing system role with provided ID or null</returns>
        Task<GrcResponse<GrcRoleGroupResponse>> GetRoleGroupByIdAsync(long recordId, long userId, string ipAddress);
        /// <summary>
        /// Get a list of role groups in the system
        /// </summary>
        /// <param name="request">Role request object</param>
        /// <returns>Task containing a list of system roles or empty list if none</returns>
        Task<GrcResponse<ListResponse<GrcRoleGroupResponse>>> GetRoleGroupsAsync(GrcRequest request);
        /// <summary>
        /// Get a list of pagednated system role groups records
        /// </summary>
        /// <param name="request">Pagenated request</param>
        /// <returns>Task containing a list of system roles or empty list if none</returns>
        Task<GrcResponse<PagedResponse<GrcRoleGroupResponse>>> GetPagedRoleGroupsAsync(TableListRequest request);
        /// <summary>
        /// Create a new system role in the system
        /// </summary>
        /// <param name="roleRecord">System role group to add</param>
        /// <param name="userId">User ID for user to logout</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns>Task containing persistance status of this role</returns>
        Task<GrcResponse<ServiceResponse>> CreateRoleGroupAsync(RoleGroupViewModel roleRecord, long userId, string ipAddress);
        /// <summary>
        /// Update system role in the system
        /// </summary>
        /// <param name="roleRecord">System role group to update</param>
        /// <param name="userId">User ID for user to look out for</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns>Task containing update status of this role</returns>
        Task<GrcResponse<ServiceResponse>> UpdateRoleGroupAsync(RoleGroupViewModel roleRecord, long userId, string ipAddress);
        /// <summary>
        /// Delete system role from the system
        /// </summary>
        /// <param name="request">System role role to delete</param>
        /// <returns>Task containing persistance status of this system role group</returns>
        Task<GrcResponse<ServiceResponse>> DeleteRoleGroupAsync(GrcIdRequest request);

        #endregion

    }

}