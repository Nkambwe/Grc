using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Services;

namespace Grc.ui.App.Areas.Operations.Helpers {
    public class DashboardCardGenerator {

        public static DashboardChartViewModel CreateCard(ILocalizationService localizationService, OperationsUnitCountResponse stats) {
            var chart = new DashboardChartViewModel();

            //..process cards
            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.TotalProcesses"),
                Value = stats.UnitProcesses.TotalUnitProcess.TotalProcesses,
                CssClass = "stat-separator-default",
                Controller = "OperationDashboard",
                Action = "TotalProcesses"
            });

            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Proposed"),
                Value = stats.UnitProcesses.ProposedProcesses.TotalProcesses,
                CssClass = "stat-separator-primary",
                Controller = "OperationDashboard",
                Action = "Proposed"
            });
            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Unchanged"),
                Value = stats.UnitProcesses.UnchangedProcesses.TotalProcesses,
                CssClass = "stat-separator-nuetral",
                Controller = "OperationDashboard",
                Action = "Unchanged"
            });
            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.DueForReview"),
                Value = stats.UnitProcesses.ProcessesDueForReview.TotalProcesses,
                CssClass = "stat-separator-danger",
                Controller = "OperationDashboard",
                Action = "Due"
            });
            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Dormant"),
                Value = stats.UnitProcesses.DormantProcesses.TotalProcesses,
                CssClass = "stat-separator-warning",
                Controller = "OperationDashboard",
                Action = "Dormant"
            });
            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Cancelled"),
                Value = stats.UnitProcesses.CancelledProcesses.TotalProcesses,
                CssClass = "stat-separator-cancelled",
                Controller = "OperationDashboard",
                Action = "Cancelled"
            });

            chart.ProcessCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Completed"),
                Value = stats.UnitProcesses.CompletedProcesses.TotalProcesses,
                CssClass = "stat-separator-cancelled",
                Controller = "OperationDashboard",
                Action = "Completed"
            });


            //..unit cards
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.AccountServices"),
                Value = stats.ProcessCategories.AccountServiceProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "AccountServices"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Cash"),
                Value = stats.ProcessCategories.CashProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Cash"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Channels"),
                Value = stats.ProcessCategories.ChannelProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Channels"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Payments"),
                Value = stats.ProcessCategories.PaymentProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Payments"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Wallets"),
                Value = stats.ProcessCategories.WalletProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Wallets"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Records"),
                Value = stats.ProcessCategories.RecordsMgtProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "RecordsManagement"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.CustomerExperience"),
                Value = stats.ProcessCategories.CustomerExperienceProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "CustomerExperience"
            });
            chart.UnitCards.Add(new StatCardViewModel {
                Title = localizationService.GetLocalizedLabel("App.Menu.Dashboard.Labels.Units.Reconciliation"),
                Value = stats.ProcessCategories.ReconciliationProcesses.Total,
                CssClass = "stat-separator-colored-pearl",
                Controller = "OperationDashboard",
                Action = "Reconciliation"
            });

            //..doughnut for category totals
            chart.CategoryTotals = new Dictionary<OperationUnit, int> {
                { OperationUnit.AccountServices, stats.ProcessCategories.AccountServiceProcesses.Total },
                { OperationUnit.Cash, stats.ProcessCategories.CashProcesses.Total },
                { OperationUnit.Channels, stats.ProcessCategories.ChannelProcesses.Total },
                { OperationUnit.Payments, stats.ProcessCategories.PaymentProcesses.Total },
                { OperationUnit.Wallets, stats.ProcessCategories.WalletProcesses.Total },
                { OperationUnit.RecordsMgt, stats.ProcessCategories.RecordsMgtProcesses.Total },
                { OperationUnit.CustomerExp, stats.ProcessCategories.CustomerExperienceProcesses.Total },
                { OperationUnit.Reconciliation, stats.ProcessCategories.ReconciliationProcesses.Total }
            };

            //..stacked bar for status summaries
            chart.StatusSummaries = new List<ProcessSummary>
            {
                new() {
                    Status = ProcessCategories.UpToDate,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Proposed,
                    Categories = new Dictionary<OperationUnit, int> {
                        {  OperationUnit.AccountServices, stats.UnitProcesses.ProposedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.ProposedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.ProposedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.ProposedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.ProposedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.ProposedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.ProposedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.ProposedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Unchanged,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Due,
                    Categories = new Dictionary<OperationUnit, int> {
                        {  OperationUnit.AccountServices, stats.UnitProcesses.ProposedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.ProposedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.ProposedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.ProposedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.ProposedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.ProposedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.ProposedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.ProposedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Cancelled,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.Review,
                    Categories = new Dictionary<OperationUnit, int> {
                        { OperationUnit.AccountServices,stats.UnitProcesses.CompletedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.CompletedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.CompletedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.CompletedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.CompletedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.CompletedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.CompletedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.CompletedProcesses.ReconciliationProcesses }
                    }
                },
                new() {
                    Status = ProcessCategories.UnitTotal,
                    Categories = new Dictionary<OperationUnit, int> {
                        {  OperationUnit.AccountServices, stats.UnitProcesses.ProposedProcesses.AccountServiceProcesses },
                        { OperationUnit.Cash, stats.UnitProcesses.ProposedProcesses.CashProcesses },
                        { OperationUnit.Channels, stats.UnitProcesses.ProposedProcesses.ChannelProcesses },
                        { OperationUnit.Payments, stats.UnitProcesses.ProposedProcesses.PaymentProcesses },
                        { OperationUnit.Wallets, stats.UnitProcesses.ProposedProcesses.WalletProcesses },
                        { OperationUnit.RecordsMgt, stats.UnitProcesses.ProposedProcesses.RecordsManagementProcesses },
                        { OperationUnit.CustomerExp, stats.UnitProcesses.ProposedProcesses.CustomerExperienceProcesses },
                        { OperationUnit.Reconciliation, stats.UnitProcesses.ProposedProcesses.ReconciliationProcesses }
                    }
                }

            };
            return chart;
        }
    }
}
