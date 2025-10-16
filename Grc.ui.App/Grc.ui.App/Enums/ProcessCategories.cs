using System.ComponentModel;

namespace Grc.ui.App.Enums
{
    public enum  ProcessCategories {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Up to date")]
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
        UnitTotal = 7
    }
}
