using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceUnitCountResponse {
        [JsonPropertyName("cashProcesses")]
        public int CashProcesses { get; set; }

        [JsonPropertyName("accountServiceProcesses")]
        public int AccountServiceProcesses { get; set; }

        [JsonPropertyName("channelProcesses")]
        public int ChannelProcesses { get; set; }

        [JsonPropertyName("paymentProcesses")]
        public int PaymentProcesses { get; set; }

        [JsonPropertyName("walletProcesses")]
        public int WalletProcesses { get; set; }

        [JsonPropertyName("recordsManagementProcesses")]
        public int RecordsManagementProcesses { get; set; }

        [JsonPropertyName("customerExperienceProcesses")]
        public int CustomerExperienceProcesses { get; set; }

        [JsonPropertyName("reconciliationProcesses")]
        public int ReconciliationProcesses { get; set; }

        [JsonPropertyName("totalProcesses")]
        public int TotalProcesses { get; set; }
    }
}
