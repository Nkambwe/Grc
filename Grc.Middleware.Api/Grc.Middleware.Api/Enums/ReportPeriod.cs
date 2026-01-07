using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {
    public enum ReportPeriod {
        [Description("NA")]
        NA = 1,
        [Description("ONE-OFF")]
        ONEOFF = 2,
        [Description("PERIODIC")]
        PERIODIC = 3,
        [Description("ON OCCURRENCE")]
        ONOCCURRENCE = 4,
        [Description("DAILY")]
        DAILY = 5,
        [Description("WEEKLY")]
        WEEKLY = 6,
        [Description("MONTHLY")]
        MONTHLY = 7,
        [Description("BIANNUAL")]
        BIANNUAL = 8,
        [Description("QUARTERLY")]
        QUARTERLY = 9,
        [Description("ANNUAL")]
        ANNUAL = 10,
        [Description("BIENNIAL")]
        BIENNIAL = 11,
        [Description("TRIENNIAL")]
        TRIENNIAL = 12
    }
}
