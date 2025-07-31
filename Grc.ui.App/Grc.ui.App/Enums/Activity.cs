using System.ComponentModel;

namespace Grc.ui.App.Enums {
    public enum Activity {
        [Description("Company Registration")]
        COMPANYREGISTRATION = 0,
        [Description("User Login")]
        LOGIN = 1,
        [Description("Login Username validation")]
        USERNAMEVALIDATION = 2,
        [Description("User authentication")]
        AUTHENTICATE = 3,
    }
}
