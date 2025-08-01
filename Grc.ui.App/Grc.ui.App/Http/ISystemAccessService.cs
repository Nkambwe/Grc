using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Http {

    public interface ISystemAccessService : IGrcBaseService {

        /// <summary>
        /// Get user info by Database ID
        /// </summary>
        /// <param name="userId">User ID to look for</param>
        /// <returns>
        /// Task containing user with provided ID or null
        /// </returns>
        Task<GrcResponse<UserModel>> GetUserByIdAsync(long userId);

        /// <summary>
        /// Get user info by user email address
        /// </summary>
        /// <param name="email">User email address to look for</param>
        /// <returns>
        /// Task containing user with provided email address or null
        /// </returns>
        Task<GrcResponse<UserModel>> GetUserByEmailAsync(string email);
        
        /// <summary>
        /// Get user info by user email address
        /// </summary>
        /// <param name="username">Username to look for</param>
        /// <returns>
        /// Task containing user with provided email address or null
        /// </returns>
        Task<GrcResponse<UserModel>> GetUserByUsernameAsync(string username);

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
        /// <returns>
        /// Task containing number of users in the system
        /// </returns>
        Task<int> CountAllUsersAsync();

        /// <summary>
        /// Get count of active users in the system
        /// </summary>
        /// <returns></returns>
        Task<int> CountActiveUsersAsync();

        /// <summary>
        /// Update the user logged in status
        /// </summary>
        /// <param name="userId">User ID for user to logout</param>
        /// <param name="isLoggedIn">Logged in status</param>
        /// <param name="ipAddress">IP Address for current user</param>
        /// <returns></returns>
        Task UpdateLoggedInStatusAsync(long userId, bool isLoggedIn, string ipAddress);
    }

}