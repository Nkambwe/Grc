namespace Grc.ui.App.Defaults{

    public class ActivityTypeDefaults {

        #region User Activities
        public const string USER_ADDED = "User.Added";
        public const string USER_EDITED = "User.Edited";
        public const string USER_DELETED = "User.Deleted";
        public const string USER_LOGIN = "User.Login";
        public const string USER_LOGOUT = "User.Logout";
        public const string USER_PASSWORD_CHANGED = "User.PasswordChanged";
        #endregion

        #region Role Activities
        public const string ROLE_ADDED = "Role.Added";
        public const string ROLE_EDITED = "Role.Edited";
        public const string ROLE_DELETED = "Role.Deleted";
        public const string ROLE_ASSIGNED_TO_USER = "Role.Assigned";
        public const string ROLE_REMOVED_FROM_USER = "Role.Revoked";
        public const string PERMISSION_SET_ADDED = "Permission.Set.Added";
        public const string PERMISSION_SET_EDITED = "Permission.Set.Edited";
        public const string PERMISSION_SET_DELETED = "Permission.Set.Deleted";
        public const string PERMISSION_SET_ASSIGNED_TO_ROLE = "Permission.Set.AssignedToRole";
        public const string PERMISSION_SET_REMOVED_FROM_ROLE = "Permission.Set.RemovedFromRole";
        #endregion

        #region Permission Activities
        public const string PERMISSION_ADDED = "Permission.Added";
        public const string PERMISSION_EDITED = "Permission.Edited";
        public const string PERMISSION_DELETED = "Permission.Deleted";
        public const string PERMISSION_ASSIGNED_TO_ROLE = "Permission.AssignedToRole";
        public const string PERMISSION_REMOVED_FROM_ROLE = "Permission.RemovedFromRole";
        #endregion

        #region System Activities
        public const string USER_SETTINGS_CHANGED = "User.Settings.Changed";
        public const string PASSWORD_POLICY_CHANGED = "Password.Policy.Changed";
        public const string DATA_ENCRYPTION_POLICY_CHANGED = "Data.Encryption.Policy.Changed";
        #endregion

    }
}
