namespace Grc.ui.App.Defaults{

    public class ActivityTypeDefaults {

        #region User Action Activities

        public const string ACTIVITY_DELETED = "System.Activity.Deleted";
        public const string ACTIVITY_RETRIEVED = "System.Activity.Retrieve";

        #endregion

        #region Departments

        public const string COMPLIACE_CREATE_DEPARTMENT = "Regulatory.Department.Added";
        public const string COMPLIANCE_EDITED_DEPARTMENT = "Regulatory.Department.Edited";
        public const string COMPLIANCE_DELETED_DEPARTMENT = "Regulatory.Department.Deleted";
        public const string COMPLIACE_CREATE_DEPARTMENT_TYPE = "Regulatory.Department.Type.Added";
        public const string COMPLIANCE_EDITED_DEPARTMENT_TYPE = "Regulatory.Department.Type.Edited";
        public const string COMPLIANCE_DELETED_DEPARTMENT_TYPE = "Regulatory.Department.Type.Deleted";

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
        public const string ROLE_PERMISSIONS_RETRIEVED = "Role.Permissions.Retrieve";
        public const string ROLE_PERMISSION_SETS_RETRIEVED = "Role.Permissions.PermissionSet.Retrieved";
        public const string ROLE_USERS_RETRIEVED = "Role.Users.Retrieved";
        #endregion

        #region Role Group Activities

        public const string ROLE_GROUP_ADDED = "Role.Group.Added";
        public const string ROLE_GROUP_ADDED_WITH_PERMISSIONS = "Role.Group.Permissions.Added";
        public const string ROLE_GROUP_EDITED = "Role.Group.Edited";
        public const string ROLE_GROUP_EDITED_WITH_PERMISSIONS = "Role.Group.Permissions.Edited";
        public const string ROLE_GROUP_DELETED = "Role.Group.Deleted";
        public const string ROLE_GROUP_ASSIGNED_TO_ROLE = "Role.Group.Assigned";
        public const string ROLE_GROUP_REMOVED_FROM_ROLE = "Role.Group.Revoked";
        public const string ROLE_GROUP_RETRIEVED = "Role.Group.Retrieved";
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

        #region Role Group Permissions

        public const string ROLE_GROUP_PERMISSIONS_ADDED = "Permission.Role.Group.Added";
        public const string ROLE_GROUP_PERMISSIONS_EDITED = "Permission.Role.Group.Edited";
        public const string ROLE_GROUP_PERMISSIONS_DELETED = "Permission.Role.Group.Deleted";
        public const string ROLE_GROUP_PERMISSIONS_RETRIEVED = "Permission.Role.Group.Retrieve";
        public const string ROLE_GROUP_PERMISSIONS_APPROVED = "Permission.Role.Group.Approved";
        public const string ROLE_GROUP_PERMISSIONS_ASSIGNED_TO_ROLE = "Permission.Role.Group.AssignedToRole";
        public const string ROLE_GROUP_PERMISSIONS_REMOVED_FROM_ROLE = "Permission.Role.Group.RemovedFromRole";

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
        public const string COMPLIANCE_LOCK_POLICY = "Regulatory.Policy.Lock";
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
        public const string COMPLIANCE_CONTROLCATEGORY_RETRIVE = "Register.Compliance.Controls.Retrieve";
        public const string COMPLIANCE_CONTROLCATEGORY_CREATE = "Register.Compliance.Controls.Create";
        public const string COMPLIANCE_CONTROLCATEGORY_UPDATE = "Register.Compliance.Controls.Update";
        public const string COMPLIANCE_CONTROLCATEGORY_DELETE = "Register.Compliance.Controls.Delete";
        public const string COMPLIANCE_CONTROLITEM_CREATE = "Register.Compliance.ControlItems.Create";
        public const string COMPLIANCE_CONTROLITEM_UPDATE = "Register.Compliance.ControlItems.Update";
        public const string COMPLIANCE_CONTROLITEM_DELETE = "Register.Compliance.ControlItems.Delete";
        public const string PROCESSES_RETRIEVE_PROCESS = "Operations.Processes.Retrieve";
        public const string PROCESSES_CREATE_PROCESS = "Operations.Processes.Create";
        public const string PROCESSES_EDITED_PROCESS = "Operations.Processes.Edit";
        public const string PROCESSES_REQUEST_APPROVAL = "Operations.Processes.Request.Approval";
        public const string PROCESSES_DELETED_PROCESS = "Operations.Processes.Delete";
        public const string PROCESSES_EXPORT_PROCESS = "Operations.Processes.Export";
        public const string PROCESSES_FILE_UPLOAD = "Operations.Processes.Fileupload";
        public const string PROCESSES_RETRIEVE_GROUP = "Operations.Processes.Groups.Retrieve";
        public const string PROCESSES_CREATE_GROUP = "Operations.Processes.Groups.Create";
        public const string PROCESSES_EDITED_GROUP = "Operations.Processes.Groups.Edit";
        public const string PROCESSES_DELETED_GROUP = "Operations.Processes.Groups.Delete";
        public const string PROCESSES_EXPORT_GROUP = "Operations.Processes.Groups.Export";
        public const string PROCESSES_RETRIEVE_TAG = "Operations.Processes.Tags.Retrieve";
        public const string PROCESSES_CREATE_TAG = "Operations.Processes.Tags.Create";
        public const string PROCESSES_EDITED_TAG = "Operations.Processes.Tags.Edit";
        public const string PROCESSES_DELETED_TAG = "Operations.Processes.Tags.Delete";
        public const string PROCESSES_EXPORT_TAG = "Operations.Processes.Tags.Export";
        public const string PROCESSES_RETRIEVE_TAT = "Operations.Processes.TAT.Retrieve";
        public const string PROCESSES_EXPORT_TAT = "Operations.Processes.TAT.Export";
        public const string APPROVAL_RETRIEVE_APPLY = "Operations.Processes.Approvals.Retrieve";
        public const string APPROVAL_RETRIEVE_RECORD = "Operations.Processes.Approvals.Record";
        public const string PROCESSES_UPDATE_APPROVAL = "Operations.Processes.Approvals.Update";
        public const string PROCESSES_INITIATE_REVIEW = "Operations.Processes.Initiate.Review";
        public const string PROCESSES_HOLD_REVIEW = "Operations.Processes.Hold.Review";
        public const string COMPLIANCE_RETRIEVE_LAWS = "Register.Laws.Retrieve";
        public const string COMPLIANCE_CREATE_LAW = "Register.Laws.Create";
        public const string COMPLIANCE_EDITED_LAW = "Register.Laws.Edited";
        public const string COMPLIANCE_DELETED_LAW = "Register.Laws.Deleted";
        public const string COMPLIANCE_RETRIEVE_ISSUE = "Register.Issues.Retrieve";
        public const string COMPLIANCE_CREATE_ISSUE = "Register.Issues.Create";
        public const string COMPLIANCE_EDITED_ISSUE = "Register.Issues.Edited";
        public const string COMPLIANCE_DELETED_ISSUE = "Register.Issues.Deleted";
        public const string COMPLIANCE_RETRIEVE_RETURN = "Returns.Report.Retrieve";
        public const string COMPLIANCE_CREATE_RETURN = "Returns.Report.Create";
        public const string COMPLIANCE_EDITED_RETURN = "Returns.Report.Edited";
        public const string COMPLIANCE_DELETED_RETURN = "Returns.Report.Deleted";

        #endregion

    }
}
