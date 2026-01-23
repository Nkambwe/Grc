using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {
    public enum ReportingPeriod {
        [Description("otal Exceptions")]
        Totals = 0,
        [Description("Due less than a Month")]
        MonthLess=1,
        [Description("Due in a Month")]
        OneMonth=2,
        [Description("Due 2 to 6 Months")]
        TwoSixMonths=3,
        [Description("Due above 6 months")]
        AboveSixMonths=4
    }
}
