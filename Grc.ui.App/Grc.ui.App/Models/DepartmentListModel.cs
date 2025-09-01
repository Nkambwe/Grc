using Grc.ui.App.Defaults;

namespace Grc.ui.App.Models {
    public class DepartmentListModel {
         public string Initials { get; set; }
         public WorkspaceModel Workspace { get; set; }
         public string Banner {get;} = CommonDefaults.AppVersion;
    }
}