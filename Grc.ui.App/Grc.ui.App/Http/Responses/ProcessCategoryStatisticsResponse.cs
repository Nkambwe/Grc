using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class ProcessCategoryStatisticsResponse {
        /// <summary>
        /// Get or set categories for cash unit
        /// </summary>
        [JsonPropertyName("cashProcesses")]
        public CategoriesCountResponse CashProcesses { get; set; }
        /// <summary>
        /// Get or set categories for account service unit
        /// </summary>
        [JsonPropertyName("accountServiceProcesses")]
        public CategoriesCountResponse AccountServiceProcesses { get; set; }
        /// <summary>
        /// Get or set categories for channel unit
        /// </summary>
        [JsonPropertyName("channelProcesses")]
        public CategoriesCountResponse ChannelProcesses { get; set; }
        /// <summary>
        /// Get or set categories for payment unit
        /// </summary>
        [JsonPropertyName("paymentProcesses")]
        public CategoriesCountResponse PaymentProcesses { get; set; }
        /// <summary>
        /// Get or set categories for wallet unit
        /// </summary>
        [JsonPropertyName("walletProcesses")]
        public CategoriesCountResponse WalletProcesses { get; set; }
        /// <summary>
        /// Get or set categories for customer experience unit
        /// </summary>
        [JsonPropertyName("customerExperienceProcesses")]
        public CategoriesCountResponse CustomerExperienceProcesses { get; set; }
        /// <summary>
        /// Get or set categories for reconciliation unit
        /// </summary>
        [JsonPropertyName("reconciliationProcesses")]
        public CategoriesCountResponse ReconciliationProcesses { get; set; }
        /// <summary>
        /// Get or set categories for reconciliation unit
        /// </summary>
        [JsonPropertyName("recordsMgtProcesses")]
        public CategoriesCountResponse RecordsMgtProcesses { get; set; }
        /// <summary>
        /// Get or set unit total process
        /// </summary>
        [JsonPropertyName("totalCategoryProcesses")]
        public CategoriesCountResponse TotalCategoryProcesses { get; set; }

    }

}
