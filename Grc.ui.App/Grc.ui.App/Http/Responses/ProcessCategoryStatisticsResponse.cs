namespace Grc.ui.App.Http.Responses
{
    public class ProcessCategoryStatisticsResponse {
        /// <summary>
        /// Get or set categories for cash unit
        /// </summary>
        public CategoriesCountResponse CashProcesses { get; set; }
        /// <summary>
        /// Get or set categories for account service unit
        /// </summary>
        public CategoriesCountResponse AccountServiceProcesses { get; set; }
        /// <summary>
        /// Get or set categories for channel unit
        /// </summary>
        public CategoriesCountResponse ChannelProcesses { get; set; }
        /// <summary>
        /// Get or set categories for payment unit
        /// </summary>
        public CategoriesCountResponse PaymentProcesses { get; set; }
        /// <summary>
        /// Get or set categories for wallet unit
        /// </summary>
        public CategoriesCountResponse WalletProcesses { get; set; }
        /// <summary>
        /// Get or set categories for customer experience unit
        /// </summary>
        public CategoriesCountResponse CustomerExperienceProcesses { get; set; }
        /// <summary>
        /// Get or set categories for reconciliation unit
        /// </summary>
        public CategoriesCountResponse ReconciliationProcesses { get; set; }
        /// <summary>
        /// Get or set categories for reconciliation unit
        /// </summary>
        public CategoriesCountResponse RecordsMgtProcesses { get; set; }
        /// <summary>
        /// Get or set unit total process
        /// </summary>
        public CategoriesCountResponse TotalCategoryProcesses { get; set; }

    }

}
