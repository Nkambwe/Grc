using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {
    public enum ServiceProcessCategories {
        [Description("Draft")]
        Draft = 0,
        [Description("UpToDate")]
        UpToDate = 1,
        [Description("Unchanged")]
        Unchanged = 2,
        [Description("Unclassified")]
        Proposed = 3,
        [Description("Obsolete")]
        Due = 4,
        [Description("Dormant")]
        Dormant = 5,
        [Description("Cancelled")]
        Cancelled = 6,
        [Description("Totals")]
        UnitTotal = 7,
        [Description("On Hold")]
        OnHold = 8,
        [Description("Review")]
        Review = 9
    }
}
