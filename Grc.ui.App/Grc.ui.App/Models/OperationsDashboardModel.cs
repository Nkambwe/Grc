using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Models {
    public class OperationsDashboardModel {
        public string Banner { get; } = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }

        public DashboardChartViewModel ChartViewModel { get; set; } = new();

        public OperationsUnitCountResponse DashboardStatistics { get; set; }
        public CategoriesCountResponse UnitStatistics { get; set; }

        public WorkspaceModel Workspace { get; set; }
        public DepartmentListModel DepartmentListModel { get; set; }
        public List<QuickActionModel> QuickActions { get; set; } = new();

    }
}