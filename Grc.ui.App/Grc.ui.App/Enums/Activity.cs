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

    }
}
