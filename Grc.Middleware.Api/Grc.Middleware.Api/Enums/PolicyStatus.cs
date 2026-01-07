using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {
    public enum PolicyStatus {
        [Description("OnHold")]
        ONHOLD = 1,
        [Description("Need Review")]
        NEEDREVIEW = 2,
        [Description("Pending Board Review")]
        PENDINGBOARD = 3,
        [Description("Pending Department Review")]
        PENDINGDEPARTMENT = 4,
        [Description("Uptodate")]
        UPTODATE = 5,
        [Description("Standard")]
        STANDARD = 6,
        [Description("Totals")]
        TOTALS = 7
    }
}
