using System.ComponentModel;

namespace Grc.ui.App.Enums {
    public enum ActionKey {
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
    }
}
