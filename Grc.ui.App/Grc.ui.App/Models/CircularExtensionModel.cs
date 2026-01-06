namespace Grc.ui.App.Models {
    public class CircularExtensionModel {
        public string Banner { get; set; }
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public Dictionary<string, int> Circulars { get; set; } = new();
        public List<QuickActionModel> QuickActions { get; set; } = new();

    }
}
