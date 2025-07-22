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
        Task<UserModel> GetUserByIdAsync(long userId);

        /// <summary>
        /// Get user info by user email address
        /// </summary>
        /// <param name="email">User email address to look for</param>
        /// <returns>
        /// Task containing user with provided email address or null
        /// </returns>
        Task<UserModel> GetUserByEmailAsync(string email);
        
        /// <summary>
        /// Get user info by user email address
        /// </summary>
        /// <param name="username">Username to look for</param>
        /// <returns>
        /// Task containing user with provided email address or null
        /// </returns>
        Task<UserModel> GetUserByUsernameAsync(string username);

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
    }

}