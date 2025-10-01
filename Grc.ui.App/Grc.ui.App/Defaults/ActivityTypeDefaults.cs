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

        #region Compliance Activities
        public const string COMPLIACE_RETRIVE_CATEGORY = "Regulatory.Category.Retrieve";
        public const string COMPLIACE_CREATE_CATEGORY = "Regulatory.Category.Added";
        public const string COMPLIANCE_EDITED_CATEGORY = "Regulatory.Category.Edited";
        public const string COMPLIANCE_DELETED_CATEGORY = "Regulatory.Category.Deleted";
        public const string COMPLIANCE_EXPORT_CATEGORY = "Regulatory.Category.Export";
        public const string COMPLIACE_CREATE_DEPARTMENT = "Regulatory.Department.Added";
        public const string COMPLIANCE_EDITED_DEPARTMENT = "Regulatory.Department.Edited";
        public const string COMPLIANCE_DELETED_DEPARTMENT = "Regulatory.Department.Deleted";
        public const string COMPLIACE_CREATE_DEPARTMENT_TYPE = "Regulatory.Department.Type.Added";
        public const string COMPLIANCE_EDITED_DEPARTMENT_TYPE = "Regulatory.Department.Type.Edited";
        public const string COMPLIANCE_DELETED_DEPARTMENT_TYPE = "Regulatory.Department.Type.Deleted";
        public const string COMPLIACE_CREATE_REGULATORY_TYPE = "Regulatory.Type.Added";
        public const string COMPLIANCE_EDITED_REGULATORY_TYPE = "Regulatory.Type.Edited";
        public const string COMPLIANCE_DELETED_REGULATORY_TYPE = "Regulatory.Type.Deleted";
        public const string COMPLIACE_CREATE_AUTHORITIES = "Regulatory.Authorities.Added";
        public const string COMPLIANCE_EDITED_AUTHORITIES = "Regulatory.Authorities.Edited";
        public const string COMPLIANCE_DELETED_AUTHORITIES = "Regulatory.Authorities.Deleted";

        public const string COMPLIANCE_EXPORT_TYPE = "Regulatory.Type.Export";
        public const string COMPLIANCE_DELETED_TYPE = "Regulatory.Type.Deleted";
        public const string COMPLIANCE_EDITED_TYPE = "Regulatory.Type.Edited";
        public const string COMPLIACE_CREATE_TYPE = "Regulatory.Type.Create";
        public const string COMPLIACE_RETRIVE_TYPE = "Regulatory.Type.Retrieve";
        #endregion

    }
}
