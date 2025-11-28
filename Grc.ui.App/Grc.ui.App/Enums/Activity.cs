using System.ComponentModel;

namespace Grc.ui.App.Enums {

    public enum Activity {
        [Description("Company Registration")]
        COMPANYREGISTRATION = 0,
        [Description("User Login")]
        LOGIN = 1,
        [Description("User Logout")]
        LOGOUT = 2,
        [Description("Login Username validation")]
        USERNAMEVALIDATION = 3,
        [Description("User authentication")]
        AUTHENTICATE = 4,
        [Description("Retrieve user record by ID")]
        RETRIVEUSERBYID = 5,
        [Description("Retrieve user record by Username")]
        RETRIVEUSERBYNAME = 6,
        [Description("Retrieve user record by email")]
        RETRIVEUSERBYEMAIL = 7,
        [Description("Retrieve record count for users")]
        COUNTUSERS = 8,
        [Description("Retrieve user statistics")]
        STATISTICS = 9,
        [Description("Get a list of branches available")]
        RETRIEVEBRANCHES = 10,
        [Description("Get a list of all available branches")]
        RETRIEVEALLBRANCHES = 11,
        [Description("Get a list of departments available")]
        RETRIEVEDEPARTMENTS = 12,
        [Description("Get a list of units available")]
        RETRIEVEUNITS = 13,
        [Description("Get a list of regulatory categories available")]
        RETRIEVEREGULATORYCATEGORIES = 14,
        [Description("Add regulatory category")]
        CREATEREGULATORYCATEGORY = 15,
        [Description("Update regulatory category")]
        COMPLIANCE_EDITED_CATEGORY = 16,
        [Description("Delete regulatory category")]
        COMPLIANCE_DELETED_CATEGORY = 17,
        [Description("Get regulatory category details")]
        COMPLIANCE_GET_CATEGORY = 18,
        [Description("Export regulatory categories")]
        COMPLIANCE_EXPORT_CATEGORY = 19,
        [Description("Get a list of regulatory types available")]
        RETRIEVEREGULATORYTYPES = 20,
        [Description("Export regulatory types")]
        COMPLIANCE_EXPORT_TYPES = 21,
        [Description("Delete regulatory type")]
        COMPLIANCE_DELETED_TYPE = 22,
        [Description("Update regulatory type")]
        COMPLIANCE_EDITED_TYPE = 23,
        [Description("Add regulatory type")]
        CREATEREGULATORYTYPE = 24,
        [Description("Get regulatory category details")]
        COMPLIANCE_GET_TYPE = 25,
        [Description("Get regulatory authority details")]
        COMPLIANCE_GET_AUTHORITY = 26,
        [Description("Get a list of regulatory authority available")]
        COMPLIANCE_RETRIEVE_AUTHORITY = 27,
        [Description("Add regulatory authority")]
        COMPLIANCE_CREATE_AUTHORITY = 28,
        [Description("Update regulatory authority")]
        COMPLIANCE_EDITED_AUTHORITY = 29,
        [Description("Delete regulatory authority")]
        COMPLIANCE_DELETED_AUTHORITY = 30,
        [Description("Export regulatory authorities")]
        COMPLIANCE_EXPORT_AUTHORITIES = 31,
        [Description("Get a list of regulatory policies available")]
        COMPLIANCE_RETRIEVE_POLICY = 32,
        [Description("Add policy/procedure")]
        COMPLIANCE_CREATE_POLICY = 33,
        [Description("Update policy/procedure")]
        COMPLIANCE_EDITED_POLICY = 34,
        [Description("Delete policy/procedure")]
        COMPLIANCE_DELETED_POLCIY = 35,
        [Description("Export policies/procedures")]
        COMPLIANCE_EXPORT_POLICIES = 36,
        [Description("Get policies/procedure details")]
        COMPLIANCE_GET_POLICY = 37,
        [Description("Get a list of document types available")]
        COMPLIANCE_RETRIEVE_DOCTYPE = 38,
        [Description("Add document type")]
        COMPLIANCE_CREATE_DOCTYPE = 39,
        [Description("Update document type")]
        COMPLIANCE_EDITED_DOCTYPE = 40,
        [Description("Delete document type")]
        COMPLIANCE_DELETED_DOCTYPE = 41,
        [Description("Export document type")]
        COMPLIANCE_EXPORT_DOCTYPES = 42,
        [Description("Get document type details")]
        COMPLIANCE_GET_DOCTYPE = 43,

        [Description("Get a list of document owners available")]
        COMPLIANCE_RETRIEVE_DOCOWNERS = 44,
        [Description("Add document owner")]
        COMPLIANCE_CREATE_DOCOWNER = 45,
        [Description("Update document owner")]
        COMPLIANCE_EDITED_DOCOWNER = 46,
        [Description("Delete document owner")]
        COMPLIANCE_DELETED_DOCOWNER = 47,
        [Description("Export document owner")]
        COMPLIANCE_EXPORT_DOCOWNERS = 48,
        [Description("Get document owner details")]
        COMPLIANCE_GET_DOCOWNER = 49,
        [Description("Retrieve all users")]
        RETRIVEUSERS = 50,
        [Description("Retrieve all tasks")]
        COMPLIANCE_RETRIEVE_TASK = 51,
        [Description("Add policy task")]
        COMPLIANCE_CREATE_TASK = 52,
        [Description("Update policy task")]
        COMPLIANCE_EDITED_TASK = 53,
        [Description("Delete policy task")]
        COMPLIANCE_DELETED_TASK = 54,
        [Description("Export policy task")]
        COMPLIANCE_EXPORT_TASK = 55,
        [Description("Get task details")]
        COMPLIANCE_GET_TASK = 56,
        [Description("Get legal act details")]
        COMPLIANCE_GET_ACTS = 57,
        [Description("Retrieve all legal acts")]
        COMPLIANCE_RETRIEVE_ACTS = 58,
        [Description("Add regulatory act")]
        COMPLIANCE_CREATE_ACTS = 59,
        [Description("Update regulatory acts")]
        COMPLIANCE_EDITED_ACTS = 60,
        [Description("Delete policy task")]
        COMPLIANCE_DELETED_ACTS = 61,
        [Description("Export regualtory acts")]
        COMPLIANCE_EXPORT_ACTS = 62,
        [Description("User retrieved")]
        USER_RETRIEVED = 63,
        [Description("User added")]
        USER_ADDED = 64,
        [Description("User editted")]
        USER_EDITED =65,
        [Description("User deleted")]
        USER_DELETED =66,
        [Description("User approved")]
        USER_APPROVED =67,
        [Description("User verified")]
        USER_VERIFIED =68,
        [Description("User password reset")]
        USER_PASSWORD_RESET =69,
        [Description("User password changed")]
        USER_PASSWORD_CHANGED = 70,
        [Description("Export user records")]
        USER_EXPORTED = 71,
        [Description("Retrieve role record by ID")]
        RETRIVEROLEBYID = 72,
        [Description("Retrieve all system roles")]
        RETRIVEROLES = 73,
        [Description("System Role added")]
        ROLE_ADDED = 74,
        [Description("System Role editted")]
        ROLE_EDITED = 75,
        [Description("System Role deleted")]
        ROLE_DELETED = 76,
        [Description("System Role approved")]
        ROLE_APPROVED = 77,
        [Description("System Role verified")]
        ROLE_VERIFIED = 78,
        [Description("Retrieve role group by ID")]
        RETRIVEROLEGROUPBYID = 79,
        [Description("Retrieve all system role groups")]
        RETRIVEROLEGROUPS = 80,
        [Description("System role group added")]
        ROLE_GROUP_ADDED = 90,
        [Description("System role group editted")]
        ROLE_GROUP_EDITED = 91,
        [Description("System role group deleted")]
        ROLE_GROUP_DELETED = 92,
        [Description("System role group approved")]
        ROLE_GROUP_APPROVED = 93,
        [Description("System role group verified")]
        ROLE_GROUP_VERIFIED = 94,
        [Description("Get a list of all system activities")]
        RETRIEVEALLSYSTEMACTIVITIES = 95,
        [Description("Permission Set added")]
        PERMISSION_SET_ADDED = 96,
        [Description("Permission Set editted")]
        PERMISSION_SET_EDITED = 97,
        [Description("Permission Set deleted")]
        PERMISSION_SET_DELETED = 98,
        [Description("Permission Set approved")]
        PERMISSION_SET_APPROVED = 99,
        [Description("Permission Set verified")]
        PERMISSION_SET_VERIFIED = 100,
        [Description("Get a list of all permission sets")]
        PERMISSION_SET_RETRIVED = 101,
        [Description("Get a list of all system permission")]
        PERMISSIONS_RETRIVED = 102,
        [Description("Retrieve operations process, guide or procedure")]
        PROCESSES_RETRIEVE_PROCESS = 103,
        [Description("Create operations process, guide or procedure")]
        PROCESSES_CREATE_PROCESS = 104,
        [Description("Edit operations process, guide or procedure")]
        PROCESSES_EDITED_PROCESS = 105,
        [Description("Delete operations process, guide or procedure")]
        PROCESSES_DELETED_PROCESS = 106,
        [Description("Export operations process, guide or procedure to excel")]
        PROCESSES_EXPORT_PROCESS = 107,
        [Description("Get process support lists")]
        RETRIVEPROCESSUPPORTITEMS = 108,
        [Description("Get a list of all process groups")]
        PROCESS_GROUPS_RETRIVED = 109,
        [Description("Retrieve process groups")]
        PROCESS_GROUP_RETRIVED = 110,
        [Description("Create process group")]
        PROCESS_GROUP_CREATE = 111,
        [Description("Update process group")]
        PROCESS_GROUP_UPDATE = 112,
        [Description("Delete process group")]
        PROCESS_GROUP_DELETE = 113,
        [Description("Get a list of all process tags")]
        PROCESS_TAGS_RETRIVED = 114,
        [Description("Retrieve process tags")]
        PROCESS_TAG_RETRIVED = 115,
        [Description("Create process tag")]
        PROCESS_TAG_CREATE = 116,
        [Description("Update process tag")]
        PROCESS_TAG_UPDATE = 117,
        [Description("Delete process tags")]
        PROCESS_TAG_DELETE = 118,
        [Description("Get a list of all process TAT data")]
        PROCESS_TATDATA_RETRIVED = 119,
        [Description("Retrieve process TAT")]
        PROCESS_TAT_RETRIVED = 120,
        [Description("Export processes to excel")]
        PROCESS_EXPORT = 121,
        [Description("Retrieve process approval record")]
        PROCESS_APPROVAL_RETRIVED = 122,
        [Description("Update process approval")]
        PROCESS_APPROVAL_UPDATE = 123,
        [Description("Unlock process for review")]
        PROCESS_REVIEW = 124,
        [Description("Hold process review")]
        PROCESS_HOLD = 125,
        [Description("Request process approval")]
        PROCESSES_REQUEST_APPROVAL = 126,
        [Description("Retrieve unit statistics")]
        PROCESSES_UNIT_STATISTIC = 127,
        [Description("Retrieve process category statistics")]
        PROCESSES_CATEGORY_STATISTIC = 128
    }
}
