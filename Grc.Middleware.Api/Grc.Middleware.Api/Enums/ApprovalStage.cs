using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {
    public enum ApprovalStage {
        [Description("Unspecified")]
        NONE = 0,
        [Description("Head Of Department Operations")]
        HOD = 1,
        [Description("Head Of Department Risk")]
        RISK = 2,
        [Description("Head Of Department Compliance")]
        COMP = 3,
        [Description("Branch Operations Manager")]
        BOM = 4,
        [Description("Head Of Department Treasury")]
        TREA = 5,
        [Description("Head Of Department Credit")]
        CRT = 6,
        [Description("Head Of Department Fintech")]
        FIN = 7
    }
}
