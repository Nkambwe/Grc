using System.ComponentModel;

namespace Grc.ui.App.Enums {

    public enum RoleCategory {
        [Description("System")]
        SYSTEM = 0,
        [Description("System Administrators")]
        ADMINISTRATOR = 1,
        [Description("Application Support")]
        APPLICATIONSUPPORT = 2,
        [Description("Operations and Services")]
        OPERATIONSERVICES = 3,
        [Description("Compliance")]
        COMPLIANCEDEPT = 4,
        [Description("Operation and Services External Users")]
        OPERATIONGUESTS = 5,
        [Description("Compliance External Users")]
        COMPLIANCEGUESTS = 6,
        [Description("Application Developer")]
        DEVELOPER = 7,
    }
}
