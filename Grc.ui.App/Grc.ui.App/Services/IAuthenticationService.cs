using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IAuthenticationService : IGrcBaseService {
        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username to look for</param>
        /// <param name="ipAddress">IP Address of the current user</param>
        /// <returns>Task containg user object</returns>
        Task<GrcResponse<UserModel>> GetUserByUsernameAsync(string username, string ipAddress);
        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="user">Login request body</param>
        /// <param name="ipAddress">IP Address</param>
        /// <returns>
        /// Task containing logged in user record
        /// </returns>
        Task<GrcResponse<UserModel>> AuthenticateAsync(LoginModel user, string ipAddress);
        /// <summary>
        /// Signin current loggedin user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        Task SignInAsync(UserModel user, bool isPersistent = false);
        /// <summary>
        /// Signout current loggedin user
        /// </summary>
        /// <returns></returns>
        Task SignOutAsync(LogoutModel logout);
        /// <summary>
        /// Get the current loggedin user
        /// </summary>
        /// <param name="ipAddress">IP Address of the current user</param>
        /// <returns></returns>
        Task<GrcResponse<UserModel>> GetCurrentUserAsync(string ipAddress);

        /// <summary>
        /// Check whether user is signed in
        /// </summary>
        /// <returns>
        /// Task containg a flag as to whether user is logged in
        /// </returns>
        Task<bool> IsSignedIn();

    }

}
