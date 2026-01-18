using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;

namespace Grc.ui.App.Models {
    public class UserHomeModel {
        public string Banner {get;} = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public UserSupportViewModel SupportItems { get; set; }
        public WorkspaceModel Workspace { get; set; }

    }
}