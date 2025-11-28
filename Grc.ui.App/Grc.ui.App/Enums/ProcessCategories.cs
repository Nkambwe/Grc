using System.ComponentModel;

namespace Grc.ui.App.Enums
{
    public enum  ProcessCategories {
        [Description("Draft")]
        Draft = 0,
        [Description("UpToDate")]
        UpToDate = 1,
        [Description("Unchanged")]
        Unchanged = 2,
        [Description("Unclassified")]
        Proposed = 3,
        [Description("Need Review")]
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
