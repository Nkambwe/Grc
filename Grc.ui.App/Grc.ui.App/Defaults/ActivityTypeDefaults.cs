namespace Grc.ui.App.Defaults{

    public class ActivityTypeDefaults {

        #region User Action Activities

        public const string ACTIVITY_DELETED = "System.Activity.Deleted";
        public const string ACTIVITY_RETRIEVED = "System.Activity.Retrieve";

        #endregion

        #region User Activities

        public const string USER_ADDED = "User.Added";
        public const string USER_EDITED = "User.Edited";
        public const string USER_DELETED = "User.Deleted";
        public const string USER_APPROVED = "User.Approved";
        public const string USER_VERIFIED = "User.Verified";
        public const string USER_RETRIEVED = "User.Retrieve";
        public const string USER_LOGIN = "User.Login";
        public const string USER_LOGOUT = "User.Logout";
        public const string USER_PASSWORD_RESET = "User.Password.Reset";
        public const string USER_PASSWORD_CHANGED = "User.Password.Changed";

        #endregion

        #region Role Activities

        public const string ROLE_ADDED = "Role.Added";
        public const string ROLE_EDITED = "Role.Edited";
        public const string ROLE_DELETED = "Role.Deleted";
        public const string ROLE_ASSIGNED_TO_USER = "Role.Assigned";
        public const string ROLE_REMOVED_FROM_USER = "Role.Revoked";
        public const string ROLE_RETRIEVED = "Role.Retrieve";
        public const string ROLE_APPROVED = "Role.Approved";
        #endregion

        #region Role Group Activities

        public const string ROLE_GROUP_ADDED = "Role.Group.Added";
        public const string ROLE_GROUP_EDITED = "Role.Group.Edited";
        public const string ROLE_GROUP_DELETED = "Role.Group.Deleted";
        public const string ROLE_GROUP_ASSIGNED_TO_ROLE = "Role.Group.Assigned";
        public const string ROLE_GROUP_REMOVED_FROM_ROLE = "Role.Group.Revoked";
        public const string ROLE_GROUP_RETRIEVED = "Role.Group.Retrieve";
        public const string ROLE_GROUP_APPROVED = "Role.Group.Approved";

        #endregion

        #region Permission Activities

        public const string PERMISSION_ADDED = "Permission.Added";
        public const string PERMISSION_EDITED = "Permission.Edited";
        public const string PERMISSION_DELETED = "Permission.Deleted";
        public const string PERMISSION_ASSIGNED_TO_ROLE = "Permission.AssignedToRole";
        public const string PERMISSION_REMOVED_FROM_ROLE = "Permission.RemovedFromRole";

        #endregion

        #region Permission Sets

        public const string PERMISSION_SET_ADDED = "Permission.Set.Added";
        public const string PERMISSION_SET_EDITED = "Permission.Set.Edited";
        public const string PERMISSION_SET_DELETED = "Permission.Set.Deleted";
        public const string PERMISSION_SET_RETRIEVED = "Permission.Set.Retrieve";
        public const string PERMISSION_SET_APPROVED = "Permission.Set.Approved";
        public const string PERMISSION_SET_ASSIGNED_TO_ROLE = "Permission.Set.AssignedToRole";
        public const string PERMISSION_SET_REMOVED_FROM_ROLE = "Permission.Set.RemovedFromRole";

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
        public const string COMPLIACE_CREATE_AUTHORITY = "Regulatory.Authority.Create";
        public const string COMPLIANCE_EDITED_AUTHORITY = "Regulatory.Authority.Edited";
        public const string COMPLIANCE_DELETED_AUTHORITY = "Regulatory.Authority.Deleted";
        public const string COMPLIACE_RETRIVE_AUTHORITY = "Regulatory.Authority.Retrieve";
        public const string COMPLIANCE_EXPORT_AUTHORITY = "Regulatory.Authority.Export";
        public const string COMPLIANCE_RETRIEVE_DOCTYPE = "Regulatory.Document.Retrieve";
        public const string COMPLIANCE_CREATE_DOCTYPE = "Regulatory.Document.Create";
        public const string COMPLIANCE_EDITED_DOCTYPE = "Regulatory.Document.Edited";
        public const string COMPLIANCE_DELETED_DOCTYPE = "Regulatory.Document.Deleted";
        public const string COMPLIANCE_EXPORT_DOCTYPE = "Regulatory.Document.Export";
        public const string COMPLIANCE_RETRIEVE_POLICY = "Regulatory.Policy.Retrieve";
        public const string COMPLIANCE_CREATE_POLICY = "Regulatory.Policy.Create";
        public const string COMPLIANCE_EDITED_POLICY = "Regulatory.Policy.Edited";
        public const string COMPLIANCE_DELETED_POLICY = "Regulatory.Policy.Deleted";
        public const string COMPLIANCE_EXPORT_POLICY = "Regulatory.Policy.Export";
        public const string COMPLIANCE_RETRIEVE_DOCOWNER = "Regulatory.Document.Owners.Retrieve";
        public const string COMPLIANCE_CREATE_DOCOWNER = "Regulatory.Document.Owners.Create";
        public const string COMPLIANCE_EDITED_DOCOWNER = "Regulatory.Document.Owners.Edited";
        public const string COMPLIANCE_DELETED_DOCOWNER = "Regulatory.Document.Owners.Deleted";
        public const string COMPLIANCE_EXPORT_DOCOWNER = "Regulatory.Document.Owners.Export";
        public const string COMPLIANCE_RETRIEVE_TASK = "Regulatory.Policy.Tasks.Retrieve";
        public const string COMPLIANCE_CREATE_TASK = "Regulatory.Policy.Tasks.Create";
        public const string COMPLIANCE_EDITED_TASK = "Regulatory.Policy.Tasks.Edited";
        public const string COMPLIANCE_DELETED_TASK = "Regulatory.Policy.Tasks.Deleted";
        public const string COMPLIANCE_EXPORT_TASK = "Regulatory.Policy.Tasks.Export";
        public const string COMPLIANCE_RETRIEVE_ACTS = "Register.Acts.Retrieve";
        public const string COMPLIANCE_CREATE_ACT = "Register.Acts.Create";
        public const string COMPLIANCE_EDITED_ACT = "Register.Acts.Edited";
        public const string COMPLIANCE_DELETED_ACT = "Register.Acts.Deleted";
        public const string COMPLIANCE_EXPORT_ACT = "Register.Acts.Export";

        #endregion

        #region Departments

        public const string COMPLIACE_CREATE_DEPARTMENT = "Regulatory.Department.Added";
        public const string COMPLIANCE_EDITED_DEPARTMENT = "Regulatory.Department.Edited";
        public const string COMPLIANCE_DELETED_DEPARTMENT = "Regulatory.Department.Deleted";
        public const string COMPLIACE_CREATE_DEPARTMENT_TYPE = "Regulatory.Department.Type.Added";
        public const string COMPLIANCE_EDITED_DEPARTMENT_TYPE = "Regulatory.Department.Type.Edited";
        public const string COMPLIANCE_DELETED_DEPARTMENT_TYPE = "Regulatory.Department.Type.Deleted";

        #endregion

    }
}
