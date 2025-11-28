namespace Grc.ui.App.Dtos {
    public record StatisticTotalResponse
    {
        public string Banner { get; set; } = string.Empty;
        public Dictionary<string, int> Categories { get; set; } = new();
    }
}
