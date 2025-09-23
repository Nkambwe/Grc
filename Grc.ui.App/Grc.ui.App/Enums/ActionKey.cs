using System.ComponentModel;

namespace Grc.ui.App.Enums {
    public enum ActionKey {
        [Description("all")]
        All,
        [Description("new")]
        New,
        [Description("edit")]
        Edit,
        [Description("delete")]
        Delete,
        [Description("import")]
        Import,
        [Description("export")]
        Export,
        [Description("search")]
        Search,
        [Description("units")]
        Units,
        [Description("ugroups")]
        UserGroups,
        [Description("ugroles")]
        UserRoles,
        [Description("departments")]
        Departments,
        [Description("separator")] 
        Separator,
        [Description("permissions")]
        Permissions,
        [Description("update")]
        Update,
        [Description("close")]
        Close,
        [Description("email")]
        Email,
        [Description("excel")]
        Excel,
    }
}
