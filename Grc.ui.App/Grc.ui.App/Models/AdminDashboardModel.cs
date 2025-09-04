using Grc.ui.App.Defaults;

namespace Grc.ui.App.Models {
    public class AdminDashboardModel {
        public string Banner {get;} = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public string MiddlwareUrl { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DeactivatedUsers { get; set; }
        public int UnApprovedUsers { get; set; }
        public int UnverifiedUsers { get; set; }
        public int DeletedUsers { get; set; }
        public int TotalBugs { get; set; }
        public int NewBugs { get; set; }
        public int BugFixes { get; set; }
        public int BugProgress { get; set; }
        public int UserReportedBugs { get; set; }
        public DateTime LastLogin { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public DepartmentListModel DepartmentListModel {get;set;}
        public List<QuickActionModel> QuickActions { get; set; } = new();
        public List<PinnedModel> PinnedItems { get; set; } = new();
        public List<RecentModel> Recents { get; set; } = new();
       
    }
}