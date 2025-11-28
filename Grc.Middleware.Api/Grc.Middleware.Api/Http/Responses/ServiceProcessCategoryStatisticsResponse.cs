using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ServiceProcessCategoryStatisticsResponse {
        /// <summary>
        /// Get or set categories for cash unit
        /// </summary>
        [JsonPropertyName("cashProcesses")]
        public ServiceCategoriesCountResponse CashProcesses { get; set; }
        /// <summary>
        /// Get or set categories for account service unit
        /// </summary>
        [JsonPropertyName("accountServiceProcesses")]
        public ServiceCategoriesCountResponse AccountServiceProcesses { get; set; }
        /// <summary>
        /// Get or set categories for channel unit
        /// </summary>
        [JsonPropertyName("channelProcesses")]
        public ServiceCategoriesCountResponse ChannelProcesses { get; set; }
        /// <summary>
        /// Get or set categories for payment unit
        /// </summary>
        [JsonPropertyName("paymentProcesses")]
        public ServiceCategoriesCountResponse PaymentProcesses { get; set; }
        /// <summary>
        /// Get or set categories for wallet unit
        /// </summary>
        [JsonPropertyName("walletProcesses")]
        public ServiceCategoriesCountResponse WalletProcesses { get; set; }
        /// <summary>
        /// Get or set categories for customer experience unit
        /// </summary>
        [JsonPropertyName("customerExperienceProcesses")]
        public ServiceCategoriesCountResponse CustomerExperienceProcesses { get; set; }
        /// <summary>
        /// Get or set categories for reconciliation unit
        /// </summary>
        [JsonPropertyName("reconciliationProcesses")]
        public ServiceCategoriesCountResponse ReconciliationProcesses { get; set; }
        /// <summary>
        /// Get or set categories for reconciliation unit
        /// </summary>
        [JsonPropertyName("recordsMgtProcesses")]
        public ServiceCategoriesCountResponse RecordsMgtProcesses { get; set; }
        /// <summary>
        /// Get or set unit total process
        /// </summary>
        [JsonPropertyName("totalCategoryProcesses")]
        public ServiceCategoriesCountResponse TotalCategoryProcesses { get; set; }
    }
}
