namespace Grc.ui.App.Dtos {
    public class AdminDashboardChartViewModel {
        public List<StatCardViewModel> Users { get; set; } = new();
        public List<StatCardViewModel> Bugs { get; set; } = new();
    }
}
