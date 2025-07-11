using Grc.ui.App.Enums;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Extensions.Http  {
     /// <summary>
    /// Session manager extension class
    /// </summary>
    public static class SessionManagerExtensions {

        private static string GetKey(SessionKeys key) => key.ToString();

        #region CurrentUser

        /// <summary>
        /// Get strongly typed current user object
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <returns>Current user object saved in session</returns>
        public static CurrentUserModel GetCurrentUser(this SessionManager sessionManager)
            => sessionManager.Get<CurrentUserModel>(GetKey(SessionKeys.CurrentUser));
    

        /// <summary>
        /// Set a strongly typed current user ovbject
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <param name="user">User object to add to</param>
        public static void SetCurrentUser(this SessionManager sessionManager, CurrentUserModel user)
         => sessionManager.Save(GetKey(SessionKeys.CurrentUser), user);

        #endregion

        #region Workspace

        /// <summary>
        /// Get strongly typed current user object
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <returns>Current user object saved in session</returns>
        public static WorkspaceModel GetWorkspace(this SessionManager sessionManager)
            => sessionManager.Get<WorkspaceModel>(GetKey(SessionKeys.Workspace));

        /// <summary>
        /// Set a strongly typed workspace ovbject
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <param name="workspace">Workspace object to add to</param>
        public static void SetWorkspace(this SessionManager sessionManager, WorkspaceModel workspace)
             => sessionManager.Save(GetKey(SessionKeys.Workspace), workspace);

        #endregion

        #region Branch

        /// <summary>
        /// Get strongly typed branch object
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <returns>Current user object saved in session</returns>
        public static BranchModel GetBranch(this SessionManager sessionManager)
            => sessionManager.Get<BranchModel>(GetKey(SessionKeys.Branch));

        /// <summary>
        /// Set a strongly typed current user object
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <param name="branch">Branch object to add to</param>
        public static void SetBranch(this SessionManager sessionManager, BranchModel branch)
            => sessionManager.Save(GetKey(SessionKeys.Branch), branch);

        #endregion

        #region UserPreferences

        /// <summary>
        /// Get strongly typed user prefferences
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <returns>User prefference object saved in session</returns>
        public static UserPreferenceModel GetUserPreferences(this SessionManager sessionManager)
               => sessionManager.Get<UserPreferenceModel>(GetKey(SessionKeys.UserPreferences));

        /// <summary>
        /// Set strongly typed user prefference object to session
        /// </summary>
        /// <param name="sessionManager">Session instance</param>
        /// <param name="preferences">User prefference object to add to session</param>
        public static void SetUserPreferences(this SessionManager sessionManager, UserPreferenceModel preferences)
               => sessionManager.Save(GetKey(SessionKeys.UserPreferences), preferences);

        #endregion

        #region LastActivity

        public static DateTime? GetLastActivity(this SessionManager sessionManager)
            => sessionManager.Get<DateTime?>(GetKey(SessionKeys.LastActivity));

        public static void SetLastActivity(this SessionManager sessionManager, DateTime lastActivity)
            => sessionManager.Save(GetKey(SessionKeys.LastActivity), lastActivity);

        public static void UpdateLastActivity(this SessionManager sessionManager)
            => sessionManager.SetLastActivity(DateTime.UtcNow);

        #endregion

    }
}
