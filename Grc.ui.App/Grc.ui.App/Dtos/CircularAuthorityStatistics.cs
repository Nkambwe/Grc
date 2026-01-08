namespace Grc.ui.App.Dtos {
    public class CircularAuthorityStatistics {
        public Dictionary<string, int> Authorities { get; set; }
        public Dictionary<string, Dictionary<string, int>> Statuses { get; set; } = new();
    }
}
