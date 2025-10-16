namespace Grc.ui.App.Dtos {
    public record DashboardRecord
    {
        public string Banner { get; set; } = string.Empty;
        public Dictionary<string, int> Categories { get; set; } = new();
    }
}
