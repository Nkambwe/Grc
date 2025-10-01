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
    }
}
