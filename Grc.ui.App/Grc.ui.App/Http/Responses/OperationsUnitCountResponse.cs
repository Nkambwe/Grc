using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class OperationsUnitCountResponse {

        [JsonPropertyName("unitProcesses")]
        public OperationsUnitStatisticsResponse UnitProcesses { get; set; }

        [JsonPropertyName("processCategories")]
        public ProcessCategoryStatisticsResponse ProcessCategories { get; set; }


    }
}
