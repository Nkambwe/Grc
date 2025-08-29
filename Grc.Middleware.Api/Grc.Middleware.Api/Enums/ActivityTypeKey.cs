using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {

    public enum ActivityTypeKey {
        [Description("Enable activity logging")]
        ENABLELOGGING = 1,
        [Description("Log support and administrtation activity")]
        LOGSUPPORTACTIVITY = 2,
        [Description("Log user activity")]
        LOGUSERACTIVITY = 3,
        [Description("Allow system to auto delete user activity")]
        ALLOWAUTODELETE = 4,
        [Description("Maximum number of days activity is kept befor auto deletion")]
        AUTODELETEMAXDAYS = 5,
        [Description("Log user IP addresses")]
        LOGUSERIPADDRESS = 6,
        [Description("List of activities excluded from logging")]
        EXCLUDEDACTIVITITIES = 7
    }

}
