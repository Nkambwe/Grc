namespace Grc.ui.App.Dtos {
    public record StatisticTotalResponse
    {
        public Dictionary<string, int> Categories { get; set; } = new();
    }
}
