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

    }
}
