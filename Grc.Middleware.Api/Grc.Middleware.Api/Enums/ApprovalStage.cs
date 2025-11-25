using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {
    public enum ApprovalStage {
        [Description("NONE")]
        NONE = 0,
        [Description("HEADOFDEPT")]
        HOD = 1,
        [Description("RISK")]
        RISK = 2,
        [Description("COMPLIANCE")]
        COMP = 3,
        [Description("BRANCHOPERATIONSMGR")]
        BOM = 4,
        [Description("TREASURY")]
        TREA = 5,
        [Description("CREDIT")]
        CRT = 6,
        [Description("FINTECH")]
        FIN = 7
    }
}
