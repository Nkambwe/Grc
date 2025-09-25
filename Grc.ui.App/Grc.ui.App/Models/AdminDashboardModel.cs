using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;

namespace Grc.ui.App.Models {
    public class AdminDashboardModel {
        public string Banner {get;} = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public string MiddlwareUrl { get; set; }
        public AdminDashboardChartViewModel Statistics { get; set; }
        public DateTime LastLogin { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public DepartmentListModel DepartmentListModel {get;set;}
        public List<QuickActionModel> QuickActions { get; set; } = new();
        public List<PinnedModel> PinnedItems { get; set; } = new();
        public List<RecentModel> Recents { get; set; } = new();
       
    }
}