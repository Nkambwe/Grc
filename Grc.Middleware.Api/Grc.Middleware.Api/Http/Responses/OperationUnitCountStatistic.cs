using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {

    public class ServiceOperationUnitCountResponse {

        [JsonPropertyName("unitProcesses")]
        public ServiceOperationUnitStatisticsResponse UnitProcesses { get; set; }

        [JsonPropertyName("processCategories")]
        public ServiceProcessCategoryStatisticsResponse ProcessCategories { get; set; }
    }
}
