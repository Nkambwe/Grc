using Grc.ui.App.Models;

namespace Grc.ui.App.Dtos {
    public class CategoryExtensionModel
    {
        public string Banner { get; set; }
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public Dictionary<string, int> CategoryProcesses { get; set; } = new();
        public List<QuickActionModel> QuickActions { get; set; } = new();

    }

}
