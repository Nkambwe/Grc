using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface ISystemAccessService : IGrcBaseService {

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
        /// Get user validation
        /// </summary>
        /// <param name="model">Login model to validate agnest</param>
        /// <returns>
        /// </returns>
        Task<GrcResponse<UsernameValidationResponse>> ValidateUsernameAsync(UsernameValidationModel model);

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
        
    }

}