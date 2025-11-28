using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Models;

namespace Grc.ui.App.Dtos {
    public class TotalExtensionModel {
        public string Banner { get; } = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public List<StatisticTotalResponse> Charts { get; set; } = new();
        public List<QuickActionModel> QuickActions { get; set; } = new();

    }

}
