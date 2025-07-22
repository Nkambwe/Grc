
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IAuthenticationService : IGrcBaseService {
        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="user">Login request body</param>
        /// <param name="ipAddress">IP Address</param>
        /// <returns>
        /// Task containing logged in user record
        /// </returns>
        Task<UserModel> AuthenticateAsync(LoginModel user, string ipAddress);
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
        Task SignOutAsync();
        /// <summary>
        /// Get the current loggedin user
        /// </summary>
        /// <returns></returns>
        Task<UserModel> GetCurrentUserAsync();

        /// <summary>
        /// Check whether user is signed in
        /// </summary>
        /// <returns>
        /// Task containg a flag as to whether user is logged in
        /// </returns>
        Task<bool> IsSignedIn();

        /// <summary>
        /// Send one time login password via mail
        /// </summary>
        /// <param name="requestModel">Request model</param>
        /// <returns></returns>
        //Task SendOnetimePasswordAsync(PasswordResetModel requestModel);
        
        /// <summary>
        /// Send in temporarly 2 factor login via phone
        /// </summary>
        /// <param name="requestModel">Request model</param>
        /// <returns></returns>
        //Task SendLoginCodeAsync(PasswordResetModel requestModel);
    }

}
