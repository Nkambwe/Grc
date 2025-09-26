namespace Grc.ui.App.Http.Responses {
    public class ComplianceStatistics {   
        public CircularTotalsStatistics CircularTotals { get; set; } = new CircularTotalsStatistics();
        public RegulationsTotalsStatistics RegulationsTotals { get; set; } = new RegulationsTotalsStatistics();
        public List<CircularAuthorityStatistics> CircularByAuthority { get; set; } = new List<CircularAuthorityStatistics>();
        public List<RegulationStatistics> RegulationStatistics { get; set; } = new List<RegulationStatistics>();
    }

}
