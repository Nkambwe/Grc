using System.ComponentModel;

namespace Grc.ui.App.Enums
{
    public enum OperationUnit {
        [Description("Unclassified")]
        Ùnknown = 0,
        [Description("Account Services")]
        AccountServices = 1,
        [Description("Cash")]
        Cash = 2,
        [Description("Channels")]
        Channels = 3,
        [Description("Customer Experience")]
        CustomerExp = 4,
        [Description("Reconciliation")]
        Reconciliation = 5,
        [Description("Records Management")]
        RecordsMgt = 6,
        [Description("Payment")]
        Payments = 7,
        [Description("E-Wallet")]
        Wallets = 8,
        [Description("Total")]
        CategoryTotal = 9
    }
}
