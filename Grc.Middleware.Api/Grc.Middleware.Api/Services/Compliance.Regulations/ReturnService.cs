using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public class ReturnService : BaseService, IReturnService {

        public ReturnService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }

        #region Statistics

        public async Task<ComplianceStatisticsResponse> GetComplianceStatisticsAsync(bool includeDeleted) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate compliance statistics", "INFO");

            var statistics = new ComplianceStatisticsResponse() {
                CircularStatuses = new(),
                ReturnStatuses = new(),
                TaskStatuses = new(),
                Policies = new()
            };

            try {

                //..policies
                var policies = await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted);
                var statusGroups = policies.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count());
                statistics.Policies.Add("On Hold", statusGroups.GetValueOrDefault("ON-HOLD", 0));
                statistics.Policies.Add("Department Review", statusGroups.GetValueOrDefault("DEPT-REVIEW", 0));
                statistics.Policies.Add("Not Uptodate", statusGroups.GetValueOrDefault("DUE", 0));
                statistics.Policies.Add("Board Review", statusGroups.GetValueOrDefault("PENDING-BOARD", 0));
                statistics.Policies.Add("MRC Review", statusGroups.GetValueOrDefault("PENDING-MRC", 0));
                statistics.Policies.Add("SMT Review", statusGroups.GetValueOrDefault("PENDING-SMT", 0));
                statistics.Policies.Add("Uptodate", statusGroups.GetValueOrDefault("UPTODATE", 0));
                statistics.Policies.Add("Standard", statusGroups.GetValueOrDefault("NA", 0));
                statistics.Policies.Add("Total", policies.Count);

                //..returns
                var returns = await uow.ReturnRepository.GetAllAsync(includeDeleted, a=> a.Frequency);
                statistics.ReturnStatuses = returns.GroupBy(d => d.Frequency?.FrequencyName?? "NA").ToDictionary(g => g.Key,g => g.Count());
                statistics.ReturnStatuses.Add("TOTALS", returns.Count);

                //..circulars
                var circulars = await uow.CircularRepository.GetAllAsync(includeDeleted, c => c.Authority, c => c.Department);
                statistics.CircularStatuses = circulars.GroupBy(d => d.Authority?.AuthorityAlias ?? "OTHER").ToDictionary(g => g.Key, g => g.Count());
                statistics.CircularStatuses.Add("TOTALS", circulars.Count);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }


            return statistics;
        }

        public async Task<PolicyDashboardResponse> GetPolicyStatisticsAsync(bool includeDeleted, PolicyStatus status) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate compliance statistics", "INFO");

            var response = new PolicyDashboardResponse {
                Statistics = new Dictionary<string, int>(),
                Policies = new List<PolicyItemResponse>()
            };

            try {
                var statusFilter = MapPolicyStatusToFilter(status);

                var documents = statusFilter != null?
                    await uow.RegulatoryDocumentRepository.GetAllAsync(p => p.Status == statusFilter, includeDeleted, p => p.Owner, p => p.Owner.Department):
                    await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted, p => p.Owner, p => p.Owner.Department);

                if (documents?.Any() == true) {
                    response.Statistics = documents
                        .GroupBy(d => d.Owner?.Department?.DepartmentName ?? "Unassigned")
                        .ToDictionary(g => g.Key, g => g.Count());

                    response.Policies = documents.Select(d => new PolicyItemResponse {
                        Id = d.Id,
                        Title = d.DocumentName,
                        OwnerId = d.ResponsibilityId,
                        Department = d.Owner?.Department?.DepartmentName,
                        ReviewDate = d.LastRevisionDate
                    }).ToList();
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate policy statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<ReturnDashboardResponse> GetReturnStatisticsAsync(bool includeDeleted, ReportPeriod period) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate returns statistics", "INFO");

            var response = new ReturnDashboardResponse {
                Statistics = new Dictionary<string, int>(),
                Reports = new List<ReturnReportResponse>()
            };

            try {
                //..get frequency name from enum
                var frequencyName = MapReturnStatusToFilter(period);

                //..get returns with submissions
                var reports = await uow.ReturnRepository.GetAllAsync(
                    p => p.Frequency.FrequencyName == frequencyName,
                    includeDeleted,
                    p => p.Owner,
                    p => p.ReturnType,
                    p => p.Frequency,
                    p => p.Submissions
                );

                //..only process returns that have submissions
                var submissions = reports?
                    .Where(r => r.Submissions != null && r.Submissions.Any())
                    .SelectMany(r => r.Submissions.Select(s => new {
                        Submission = s,
                        Return = r
                    }))
                    .ToList();

                if (submissions?.Any() == true) {
                    //..department statistics based on submissions
                    response.Statistics = submissions
                        .GroupBy(x => x.Return.Owner?.ContactPosition ?? "Unassigned")
                        .ToDictionary(g => g.Key, g => g.Count());

                    //..submission-based report list
                    response.Reports = submissions.Select(x => new ReturnReportResponse {
                        Id = x.Submission.Id,              
                        Title = x.Return.ReturnName,        
                        Department = x.Return.Owner?.ContactPosition,
                        RequiredDate = x.Return.RequiredSubmissionDate,
                        RequiredDay = x.Return.RequiredSubmissionDay,
                        Type = x.Return.ReturnType?.TypeName
                    }).ToList();
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity(
                    $"Failed to generate returns statistics: {ex.Message}",
                    "ERROR");

                LogError(uow, ex);
                throw;
            }
        }

        public async Task<ReturnExtensionResponse> GetReturnExtensionStatisticsAsync(bool includeDeleted, ReportPeriod period) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate returns statistics", "INFO");

            try {
                var frequencyName = MapReturnStatusToFilter(period);
                var reports = await uow.ReturnRepository.GetAllAsync(r => r.Frequency.FrequencyName == frequencyName, includeDeleted,
                    r => r.Owner, r => r.Frequency, r => r.Submissions);

                if (reports == null || reports.Count == 0) {
                    return new ReturnExtensionResponse {
                        Periods = new Dictionary<string, int>(),
                        Reports = new List<ReturnSubmissionResponse>()
                    };
                }

                var response = new ReturnExtensionResponse {
                    Periods = reports.GroupBy(r => r.Frequency?.FrequencyName ?? "NA").ToDictionary(g => g.Key, g => g.Count()),

                    //..select only those that belobg to the period, and those breached and still open
                    Reports = reports.SelectMany(report => report.Submissions ?? Enumerable.Empty<ReturnSubmission>(), (r, s) => new { r, s })
                        .Where(record => BelongsToPeriod(record.s, period) || (record.s.IsBreached && string.Equals(record.s.Status, "OPEN", StringComparison.OrdinalIgnoreCase)))
                        .Select(set => new ReturnSubmissionResponse {
                            Id = set.s.Id,
                            Title = set.r.ReturnName,
                            PeriodStart = set.s.PeriodStart,
                            PeriodEnd = set.s.PeriodEnd,
                            Status = set.s.IsBreached ? "BREACHED" : set.s.Status ?? "OPEN",
                            RequiredSubmissionDate = set.r.RequiredSubmissionDate,
                            RequiredSubmissionDay = set.r.RequiredSubmissionDay,
                            Department = set.r.Owner?.ContactPosition,
                            Risk = set.r.Risk?.ToString()
                        }).ToList()

                };

                return response;
            } catch (Exception ex) {
                Logger.LogActivity(
                    $"Failed to generate returns statistics: {ex.Message}",
                    "ERROR");

                LogError(uow, ex);
                throw;
            }
        }

        public async Task<ReturnsStatisticsResponses> GetReturnDashboardStatisticsAsync(bool includeDeleted) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate compliance statistics", "INFO");

            var statistics = new ReturnsStatisticsResponses() {
                Periods = new Dictionary<string, int>(),
                Statuses = new Dictionary<string, Dictionary<string, int>> ()
            };

            try {
                // ..returns
                var returns = await uow.ReturnRepository.GetAllAsync(includeDeleted,r => r.Frequency,r => r.Submissions);
                statistics.Periods = returns.GroupBy(d => d.Frequency?.FrequencyName ?? "NA").ToDictionary(g => g.Key, g => g.Count());
                statistics.Periods.Add("TOTALS", returns.Count);

                //..graph data from submissions
                statistics.Statuses = returns
                .GroupBy(r => r.Frequency?.FrequencyName ?? "NA")
                .ToDictionary(
                    periodGroup => periodGroup.Key,
                    periodGroup => {
                        var reportPeriod = MapFrequencyToReportPeriod(periodGroup.Key);

                        return periodGroup
                            .SelectMany(r => r.Submissions ?? Enumerable.Empty<ReturnSubmission>())
                            .Where(s => BelongsToPeriod(s, reportPeriod) || (s.IsBreached && string.Equals(s.Status, "OPEN", StringComparison.OrdinalIgnoreCase)))
                            .GroupBy(s => s.IsBreached ? "BREACHED" : s.Status ?? "OPEN")
                            .ToDictionary(
                                statusGroup => statusGroup.Key,
                                statusGroup => statusGroup.Count()
                            );
                    }
                );

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }


            return statistics;
        }

        public async Task<CircularStatisticsResponse> GetCircularDashboardStatisticsAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Circular statistics", "INFO");

            try {
                //..fetch circulars once
                var circulars = await uow.CircularRepository.GetAllAsync(
                    includeDeleted,
                    c => c.Authority,
                    c => c.Department
                );

                var circularsList = circulars.ToList();

                //..build statistics in a single pass where possible
                var statistics = new CircularStatisticsResponse {
                    Authorities = new Dictionary<string, int>(),
                    Statuses = new Dictionary<string, Dictionary<string, int>>()
                };

                //..group by authority and calculate both metrics
                var authorityGroups = circularsList
                    .GroupBy(c => c.Authority?.AuthorityAlias ?? "OTHER")
                    .ToList();

                foreach (var authorityGroup in authorityGroups) {
                    var circularArray = authorityGroup.ToList();

                    //..count for Authorities dictionary
                    statistics.Authorities[authorityGroup.Key] = circularArray.Count;

                    //..group by status for Statuses dictionary
                    statistics.Statuses[authorityGroup.Key] = circularArray
                        .GroupBy(c => GetCircularStatus(c))
                        .ToDictionary(
                            statusGroup => statusGroup.Key,
                            statusGroup => statusGroup.Count()
                        );
                }

                //..add total count
                statistics.Authorities["TOTALS"] = circularsList.Count;

                return statistics;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<CircularDashboardResponse> GetCircularStatisticsAsync(bool includeDeleted, string authority) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate circular statistics", "INFO");

            var response = new CircularDashboardResponse {
                Statistics = new Dictionary<string, int>(),
                Circulars = new List<CircularReportResponse>()
            };

            try {

                //..get records
                var reports = await uow.CircularRepository.GetAllAsync(p => p.Authority.AuthorityAlias == authority, includeDeleted,p => p.Department, p => p.Authority);
                if (reports?.Any() == true) {
                    //..department statistics
                    response.Statistics = reports.GroupBy(d => d.Status ?? "OPEN")
                                                 .ToDictionary(g => g.Key, g => g.Count());

                    //..returns list
                    response.Circulars = reports.Select(r => new CircularReportResponse {
                        Id = r.Id,
                        Title = r.CircularTitle,
                        Status = r.IsBreached ? "BREACHED" : r.Status,
                        BreachRisk = r.BreachRisk,
                        AuthorityAlias = r.Authority?.AuthorityAlias,
                        Authority = r.Authority?.AuthorityName ?? string.Empty,
                        Department = r.Department?.DepartmentName ?? string.Empty,
                    }).ToList();
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity(
                    $"Failed to generate returns statistics: {ex.Message}",
                    "ERROR");

                LogError(uow, ex);
                throw;
            }
        }

        public async Task<CircularExtensionResponses> GetCircularExtensionStatisticsAsync(bool includeDeleted, string authority) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate circular statistics", "INFO");

            var statistics = new CircularExtensionResponses() {
                Statuses = new(),
                Reports = new()
            };

            try {
                //..get records
                var reports = await uow.CircularRepository.GetAllAsync(p => p.Authority.AuthorityAlias == authority, includeDeleted, p => p.Department, p => p.Authority);
                if (reports?.Any() == true) {
                    //..department statistics
                    statistics.Statuses = reports.GroupBy(d => d.Status ?? "OPEN")
                                                 .ToDictionary(g => g.Key, g => g.Count());

                    //..returns list
                    statistics.Reports = reports.Select(r => new CircularExtensionResponse {
                        Id = r.Id,
                        Title = r.CircularTitle,
                        Status = r.IsBreached ? "BREACHED": r.Status,
                        AuthorityAlias = r.Authority?.AuthorityAlias,
                        SubmissionDate = r.SubmissionDate,
                        DueDate = r.RequiredSubmissionDate,
                        Authority = r.Authority?.AuthorityName ?? string.Empty,
                        Department = r.Department?.DepartmentName ?? string.Empty,
                    }).ToList();
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }

            return statistics;

        }

        #endregion

        #region Reports

        public async Task<List<SummeryReturnResponse>> GetPeriodReportAsync(ReportPeriod period, bool includeDeleted) {
            
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate returns period report data", "INFO");

            try {
                var frequencyName = MapReturnStatusToFilter(period);
                var reports = await uow.ReturnRepository.GetAllAsync(r => r.Frequency.FrequencyName == frequencyName, includeDeleted,
                    r => r.Owner, r => r.Frequency, r => r.ReturnType, r => r.Authority, r => r.Submissions);

                if (reports == null || reports.Count == 0) {
                    return new List<SummeryReturnResponse>();
                }

                var response = reports.SelectMany(report => report.Submissions ?? Enumerable.Empty<ReturnSubmission>(), (setReport, setSubmission) => new { setReport, setSubmission })
                        .Where(record => BelongsToPeriod(record.setSubmission, period) || (record.setSubmission.IsBreached && string.Equals(record.setSubmission.Status, "OPEN", StringComparison.OrdinalIgnoreCase)))
                        .Select(set => new SummeryReturnResponse {
                            Id = set.setSubmission.Id,
                            Title = set.setReport.ReturnName,
                            Type = set.setReport.ReturnType?.TypeName,
                            Frequency = set.setReport.Frequency?.FrequencyName,
                            Authority = set.setReport.Authority?.AuthorityName,
                            PeriodStart = set.setSubmission.PeriodStart,
                            PeriodEnd = set.setSubmission.PeriodEnd,
                            Status = set.setSubmission.Status ?? "OPEN",
                            Department = set.setReport.Owner?.ContactPosition,
                            Executioner = set.setSubmission.SubmittedBy ?? string.Empty,
                            IsBreached = set.setSubmission.IsBreached,
                            BreachRisk = set.setReport.Risk ?? string.Empty,
                            BreachReason = set.setSubmission.BreachReason
                        }).ToList();

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate returns statistics: {ex.Message}","ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<List<PeriodSummaryResponse>> GetMonthlySummeryAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate period summary percentage report", "INFO");
            try {
                var reports = await uow.ReturnRepository.GetAllAsync(false,
                    r => r.Owner,
                    r => r.Frequency,
                    r => r.ReturnType,
                    r => r.Authority,
                    r => r.Submissions);

                if (reports == null || reports.Count == 0) {
                    return new List<PeriodSummaryResponse>();
                }

                var now = DateTime.UtcNow;
                var monthStart = new DateTime(now.Year, now.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var allowedFrequencies = new[] { "DAILY", "WEEKLY", "MONTHLY"};
                var monthlySubmissions = reports
                    .Where(r => allowedFrequencies.Contains(r.Frequency?.FrequencyName, StringComparer.OrdinalIgnoreCase))
                    .SelectMany(r => r.Submissions ?? Enumerable.Empty<ReturnSubmission>(),
                        (r, s) => new { Report = r, Submission = s })
                    .Where(submission => submission.Submission.PeriodStart < monthEnd &&
                                        submission.Submission.PeriodEnd >= monthStart)
                    .ToList();

                var summary = monthlySubmissions
                    .GroupBy(submission => submission.Report.Frequency.FrequencyName.ToUpperInvariant())
                    .Select(g => {
                        var total = g.Count();
                        var submitted = g.Count(s => string.Equals(s.Submission.Status, "CLOSED", StringComparison.OrdinalIgnoreCase));
                        var pending = g.Count(s => string.Equals(s.Submission.Status, "OPEN", StringComparison.OrdinalIgnoreCase));
                        var breached = g.Count(s => s.Submission.IsBreached);
                        var onTime = g.Count(s => !s.Submission.IsBreached &&
                                                 string.Equals(s.Submission.Status, "CLOSED", StringComparison.OrdinalIgnoreCase));

                        return new PeriodSummaryResponse {
                            Period = g.Key,
                            Total = total,
                            Submitted = submitted,
                            SubmittedPercentage = CalculatePercentage(submitted, total),
                            Pending = pending,
                            PendingPercentage = CalculatePercentage(pending, total),
                            Breached = breached,
                            BreachedPercentage = CalculatePercentage(breached, total),
                            OnTime = onTime,
                            OnTimePercentage = CalculatePercentage(onTime, total),
                            ComplianceRate = CalculatePercentage(onTime, total)
                        };
                    }).OrderBy(s => s.Period).ToList();

                return summary;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate period summary percentage report: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<List<BreachResponse>> GetBreachedReportAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate breached reports for current month", "INFO");
            try {
                var reports = await uow.ReturnRepository.GetAllAsync(false,
                    r => r.Owner,
                    r => r.Frequency,
                    r => r.ReturnType,
                    r => r.Authority,
                    r => r.Submissions);

                if (reports == null || reports.Count == 0) {
                    return new List<BreachResponse>();
                }

                var now = DateTime.UtcNow;
                var monthStart = new DateTime(now.Year, now.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                //..allowed frequencies
                var allowedFrequencies = new[] {
                    "DAILY",
                    "WEEKLY",
                    "MONTHLY"
                };

                //..get submissions for the current month
                var monthlySubmissions = reports
                    .Where(r => allowedFrequencies.Contains(r.Frequency?.FrequencyName, StringComparer.OrdinalIgnoreCase))
                    .SelectMany(report => report.Submissions ?? Enumerable.Empty<ReturnSubmission>(), (set_report, set_submission) 
                        => new { 
                            Report = set_report, 
                            Submission = set_submission
                        })
                    .Where(set => set.Submission.PeriodStart < monthEnd && set.Submission.PeriodEnd >= monthStart)
                    .ToList();

                //..filter for breached submissions and map to response
                var breachedReports = monthlySubmissions
                    .Where(report => report.Submission.IsBreached == true)
                    .Select(breach => new BreachResponse {
                        ReportName = breach.Report.ReturnType?.TypeName ?? string.Empty,
                        Frequency = breach.Report.Frequency?.FrequencyName ?? string.Empty,
                        Department = breach.Report.Owner?.ContactPosition ?? string.Empty,
                        DueDate = breach.Submission.PeriodEnd,
                        SubmissionDate = breach.Submission.SubmissionDate,
                        Status = breach.Submission.Status ?? "OPEN",
                        AssociatedRisk = breach.Report.Risk ?? "Not applicable" 
                    })
                    .ToList();

                Logger.LogActivity($"Found {breachedReports.Count} breached reports for current month", "INFO");
                return breachedReports;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate breached reports: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<List<BreachAgeResponse>> GetBreachedAgingReportAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate breached aging report for current month", "INFO");
            try {
                var reports = await uow.ReturnRepository.GetAllAsync(includeDeleted,
                    r => r.Owner,
                    r => r.Frequency,
                    r => r.ReturnType,
                    r => r.Authority,
                    r => r.Submissions);

                if (reports == null || reports.Count == 0) {
                    return new List<BreachAgeResponse>();
                }

                var now = DateTime.UtcNow;
                var monthStart = new DateTime(now.Year, now.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                //..frequencies to include
                var allowedFrequencies = new[] { "DAILY", "WEEKLY", "MONTHLY" };

                var monthlySubmissions = reports
                    .Where(r => allowedFrequencies.Contains(r.Frequency?.FrequencyName, StringComparer.OrdinalIgnoreCase))
                    .SelectMany(report => report.Submissions ?? Enumerable.Empty<ReturnSubmission>(),
                        (set_report, set_submission) 
                        => new {
                            Report = set_report,
                            Submission = set_submission
                        })
                    .Where(set => set.Submission.PeriodStart < monthEnd && set.Submission.PeriodEnd >= monthStart).ToList();
                var breachedAgingReport = monthlySubmissions
                    .Where(report => report.Submission.IsBreached == true)
                    .Select(breach => {
                        var daysOverdue = CalculateDaysOverdue(breach.Submission.PeriodEnd, breach.Submission.SubmissionDate,now);
                        return new BreachAgeResponse {
                            ReportName = breach.Report.ReturnType?.TypeName ?? string.Empty,
                            Frequency = breach.Report.Frequency?.FrequencyName ?? string.Empty,
                            Department = breach.Report.Owner?.ContactPosition ?? string.Empty,
                            DueDate = breach.Submission.PeriodEnd,
                            SubmissionDate = breach.Submission.SubmissionDate,
                            Status = breach.Submission.Status ?? "OPEN",
                            DaysOverdue = daysOverdue,
                            AgingBucket = GetAgingBucket(daysOverdue),
                            AssociatedRisk = breach.Report.Risk ?? "Not applicable"
                        };
                    })
                    .OrderByDescending(x => x.DaysOverdue)
                    .ToList();

                Logger.LogActivity($"Found {breachedAgingReport.Count} breached reports for current month", "INFO");
                return breachedAgingReport;

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate breached aging report: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<List<CircularReportResponse>> GetCircularAuthorityReportAsync(bool includeDeleted, string authority) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate circular statistics", "INFO");

            var response = new List<CircularReportResponse>();
            try {
                //..get records
                var excludedAuthorities = new[] { "BOU", "URA", "DPF", "MoFED", "PPDA" };

                var reports = authority switch {
                    "OTHER" => await uow.CircularRepository.GetAllAsync(
                        p => !excludedAuthorities.Contains(p.Authority.AuthorityAlias),
                        includeDeleted,
                        p => p.Department,
                        p => p.Authority),
                    _ => await uow.CircularRepository.GetAllAsync(
                        p => p.Authority.AuthorityAlias == authority,
                        includeDeleted,
                        p => p.Department,
                        p => p.Authority)
                };

                if (reports?.Any() == true) {
                    response = reports.Select(r => new CircularReportResponse {
                        Id = r.Id,
                        Title = r.CircularTitle,
                        Status = r.Status,
                        IsBreached = r.IsBreached,
                        DueDate = r.DeadlineOn,
                        SubmissionDate = r.SubmissionDate,
                        AuthorityAlias = r.Authority?.AuthorityAlias,
                        Authority = r.Authority?.AuthorityName ?? string.Empty,
                        Department = r.Department?.DepartmentName ?? string.Empty,
                        BreachRisk = r.BreachRisk
                    }).ToList();
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve circular report data: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
            return response;
        }

        public async Task<List<CircularSummaryResponse>> GetCircularSummeryReportAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate circular authority statistics report", "INFO");
            try {
                var currentYear = DateTime.UtcNow.Year;
                var yearStart = new DateTime(currentYear, 1, 1);
                var yearEnd = new DateTime(currentYear, 12, 31, 23, 59, 59);

                //..get all circulars received
                var circulars = await uow.CircularRepository.GetAllAsync(p => p.RecievedOn >= yearStart &&
                p.RecievedOn <= yearEnd, includeDeleted,p => p.Department, p => p.Authority);

                if (circulars == null || !circulars.Any()) {
                    Logger.LogActivity("No circulars found for current year", "INFO");
                    return new List<CircularSummaryResponse>();
                }

                //..group by authority
                var stats = circulars
                    .Where(c => c.Authority != null)
                    .GroupBy(c => new { Alias = c.Authority.AuthorityAlias, Name = c.Authority.AuthorityName})
                    .Select(g => {
                        var totalReceived = g.Count();
                        var closedCount = g.Count(c => string.Equals(c.Status, "CLOSED", StringComparison.OrdinalIgnoreCase));
                        var outstandingCount = g.Count(c => string.Equals(c.Status, "OPEN", StringComparison.OrdinalIgnoreCase) ||
                                               string.Equals(c.Status, "UNKNOWN", StringComparison.OrdinalIgnoreCase));
                        var breachedCount = g.Count(c => c.IsBreached);
                        var closedNotBreached = closedCount - g.Count(c => c.IsBreached &&  string.Equals(c.Status, "CLOSED", StringComparison.OrdinalIgnoreCase));

                        return new CircularSummaryResponse { 
                            AuthorityAlias = g.Key.Alias,
                            AuthorityName = g.Key.Name ?? string.Empty,
                            TotalReceived = totalReceived,
                            Closed = new CircularMetric {
                                Count = closedCount,
                                Percentage = CalculateCircularPercentage(closedCount, totalReceived)
                            },
                            Outstanding = new CircularMetric {
                                Count = outstandingCount,
                                Percentage = CalculateCircularPercentage(outstandingCount, totalReceived)
                            },
                            Breached = new CircularMetric {
                                Count = breachedCount,
                                Percentage = CalculateCircularPercentage(breachedCount, totalReceived)
                            },
                            ComplianceRate = CalculateCircularPercentage(closedNotBreached, totalReceived)
                        };
                    }).OrderByDescending(s => s.TotalReceived).ToList();

                Logger.LogActivity($"Generated statistics for {stats.Count} authorities", "INFO");
                return stats;

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate circular authority statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        #endregion

        #region Queries
        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database", "INFO");

            try {
                return uow.ReturnRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to Regulatory Return in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public int Count(Expression<Func<ReturnReport, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database", "INFO");

            try
            {
                return uow.ReturnRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database", "INFO");

            try
            {
                return await uow.ReturnRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of  Regulatory Returns in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ReturnReport, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ReturnRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ReturnRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Exists(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Return exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ReturnRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Return in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Return exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ReturnRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ReturnReport, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Regulatory Return if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.ReturnRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public ReturnReport Get(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return with ID '{id}'", "INFO");

            try {
                return uow.ReturnRepository.Get(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public ReturnReport Get(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.ReturnRepository.Get(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public ReturnReport Get(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.ReturnRepository.Get(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IQueryable<ReturnReport> GetAll(bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return", "INFO");

            try {
                return uow.ReturnRepository.GetAll(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IList<ReturnReport> GetAll(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all regulatory return", "INFO");

            try {
                return uow.ReturnRepository.GetAll(includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IList<ReturnReport> GetAll(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return uow.ReturnRepository.GetAll(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(includeDeleted);
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Regulatory Return", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ReturnReport> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return with ID '{id}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ReturnReport> GetAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ReturnReport> GetAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetTopAsync(Expression<Func<ReturnReport, bool>> predicate, int top, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Regulatory Returns that fit predicate >> {predicate}", "INFO");

            try {
                return await uow.ReturnRepository.GetTopAsync(predicate, top, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Insert(ReturnRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map Regulatory Return request to Regulatory Return entity
                var article = new ReturnReport() {
                    ArticleId = request.ArticleId,
                    ReturnName = request.ReturnName,
                    TypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    FrequencyId = request.FrequencyId,
                    OwnerId = request.DepartmentId,
                    Risk = request.Risk,
                    Interval = request.Interval,
                    IntervalType = request.IntervalType,
                    SendReminder = request.SendReminder,
                    Reminder = request.Reminder,
                    RequiredSubmissionDate = request.RequiredSubmissionDate,
                    RequiredSubmissionDay = request.RequiredSubmissionDay,
                    CreatedBy = request.UserName,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = request.UserName,
                    LastModifiedOn = DateTime.Now,
                    Comments = request.Comments,
                    IsDeleted = request.IsDeleted
                };

                //..log the Regulatory Return data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {articleJson}", "DEBUG");

                var added = uow.ReturnRepository.Insert(article);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save submission : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> InsertAsync(ReturnRequest request) {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Return request to Regulatory Return entity
                var article = new ReturnReport() {
                    ArticleId = request.ArticleId,
                    ReturnName = request.ReturnName,
                    TypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    FrequencyId = request.FrequencyId,
                    OwnerId = request.DepartmentId,
                    Risk = request.Risk,
                    Interval = request.Interval,
                    IntervalType = request.IntervalType,
                    SendReminder = request.SendReminder,
                    Reminder = request.Reminder,
                    RequiredSubmissionDate = request.RequiredSubmissionDate,
                    RequiredSubmissionDay = request.RequiredSubmissionDay,
                    CreatedBy = request.UserName,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = request.UserName,
                    LastModifiedOn = DateTime.Now,
                    Comments = request.Comments,
                    IsDeleted = request.IsDeleted
                };

                //..log the Regulatory Return data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {articleJson}", "DEBUG");

                var added = await uow.ReturnRepository.InsertAsync(article);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Update(ReturnRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Return request", "INFO");

            try {
                var regReturn = uow.ReturnRepository.Get(a => a.Id == request.Id);
                if (regReturn != null) {
                    //..update record
                    regReturn.ReturnName = (request.ReturnName ?? string.Empty).Trim();
                    regReturn.TypeId = request.TypeId;
                    regReturn.FrequencyId = request.FrequencyId;
                    regReturn.ArticleId = request.ArticleId;
                    regReturn.AuthorityId = request.AuthorityId;
                    regReturn.OwnerId = request.DepartmentId;
                    regReturn.Risk = request.Risk;
                    regReturn.Interval = request.Interval;
                    regReturn.IntervalType = request.IntervalType;
                    regReturn.SendReminder = request.SendReminder;
                    regReturn.Reminder = request.Reminder;
                    regReturn.RequiredSubmissionDate = request.RequiredSubmissionDate;
                    regReturn.RequiredSubmissionDay = request.RequiredSubmissionDay;
                    regReturn.Comments = (request.Comments ?? string.Empty).Trim();
                    regReturn.IsDeleted = request.IsDeleted;
                    regReturn.LastModifiedOn = DateTime.Now;
                    regReturn.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = uow.ReturnRepository.Update(regReturn, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(regReturn).State;
                    Logger.LogActivity($"Regulatory Return state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Regulatory Return record: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ReturnRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Return", "INFO");

            try {
                var regReturn = await uow.ReturnRepository.GetAsync(a => a.Id == request.Id);
                if (regReturn != null) {
                    //..update record
                    regReturn.ReturnName = (request.ReturnName ?? string.Empty).Trim();
                    regReturn.TypeId = request.TypeId;
                    regReturn.FrequencyId = request.FrequencyId;
                    regReturn.ArticleId = request.ArticleId;
                    regReturn.AuthorityId = request.AuthorityId;
                    regReturn.OwnerId = request.DepartmentId;
                    regReturn.Risk = request.Risk;
                    regReturn.Interval = request.Interval;
                    regReturn.IntervalType = request.IntervalType;
                    regReturn.SendReminder = request.SendReminder;
                    regReturn.Reminder = request.Reminder;
                    regReturn.RequiredSubmissionDate = request.RequiredSubmissionDate;
                    regReturn.RequiredSubmissionDay = request.RequiredSubmissionDay;
                    regReturn.Comments = (request.Comments ?? string.Empty).Trim();
                    regReturn.IsDeleted = request.IsDeleted;
                    regReturn.LastModifiedOn = DateTime.Now;
                    regReturn.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = await uow.ReturnRepository.UpdateAsync(regReturn, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(regReturn).State;
                    Logger.LogActivity($"Regulatory Regulatory Return state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Regulatory Return record: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {auditJson}", "DEBUG");

                var statute = uow.ReturnRepository.Get(t => t.Id == request.RecordId);
                if (statute != null)
                {
                    //..mark as delete this Regulatory Return
                    _ = uow.ReturnRepository.Delete(statute, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var statuteJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {statuteJson}", "DEBUG");

                var tasktask = await uow.ReturnRepository.GetAsync(t => t.Id == request.RecordId);
                if (tasktask != null)
                {
                    //..mark as delete this Regulatory Return
                    _ = await uow.ReturnRepository.DeleteAsync(tasktask, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(tasktask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();
            try {
                var statutes = await uow.ReturnRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (statutes.Count == 0) {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.ReturnRepository.DeleteAllAsync(statutes, markAsDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Return: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(ReturnRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map Regulatory Return to Regulatory Return entity
                var returns = requestItems.Select(Mapper.Map<ReturnRequest, ReturnReport>).ToArray();
                var returnsJson = JsonSerializer.Serialize(returns, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {returnsJson}", "DEBUG");
                return await uow.ReturnRepository.BulkyInsertAsync(returns);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ReturnRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map  Regulatory Returns request to  Regulatory Returns entity
                var returns = requestItems.Select(Mapper.Map<ReturnRequest, ReturnReport>).ToArray();
                var returnsJson = JsonSerializer.Serialize(returns, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {returnsJson}", "DEBUG");
                return await uow.ReturnRepository.BulkyUpdateAsync(returns);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ReturnRequest[] requestItems, params Expression<Func<ReturnReport, object>>[] propertySelectors) {
            using var uow = UowFactory.Create();
            try {
                //..map regulatory return request to regulatory return entity
                var returns = requestItems.Select(Mapper.Map<ReturnRequest, ReturnReport>).ToArray();
                var returnsJson = JsonSerializer.Serialize(returns, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($" Regulatory Returns data: {returnsJson}", "DEBUG");
                return await uow.ReturnRepository.BulkyUpdateAsync(returns, propertySelectors);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save  Regulatory Returns : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");

            try {
                return await uow.ReturnRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");

            try {
                return await uow.ReturnRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieves regulatory return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnReport, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");
            try {
                return await uow.ReturnRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }
        
        public async Task<PagedResult<ReturnReport>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ReturnReport, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");
            try {
                return await uow.ReturnRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnReport, bool>> predicate = null, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");
            try {
                return await uow.ReturnRepository.PageAllAsync(page, size, includeDeleted,predicate,includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        #endregion

        #region private methods

        private static double CalculatePercentage(int count, int total) 
            => total == 0 ? 0 : Math.Round((double)count / total * 100, 2);

        private static int CalculateDaysOverdue(DateTime dueDate, DateTime? submissionDate, DateTime now) {
            // If submitted late, calculate days between due date and submission
            if (submissionDate.HasValue && submissionDate.Value > dueDate) {
                return (submissionDate.Value.Date - dueDate.Date).Days;
            }
            // If not yet submitted, calculate days from due date to now
            if (!submissionDate.HasValue) {
                return (now.Date - dueDate.Date).Days;
            }
            // Shouldn't happen for breached reports, but return 0 as fallback
            return 0;
        }

        private static string GetAgingBucket(int daysOverdue) {
            return daysOverdue switch {
                <= 7 => "1-7 days",
                <= 14 => "8-14 days",
                <= 30 => "15-30 days",
                <= 60 => "31-60 days",
                <= 90 => "61-90 days",
                _ => "90+ days"
            };
        }

        private static (DateTime Start, DateTime End) GetCurrentPeriodRange(string frequency, DateTime now) {
            now = now.Date;
            return frequency?.ToUpperInvariant() switch {
                "DAILY" => (now,now.AddDays(1)),
                "WEEKLY" => (now.AddDays(-(int)now.DayOfWeek),   now.AddDays(7 - (int)now.DayOfWeek)),
                "MONTHLY" => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1)),
                "QUARTERLY" => (new DateTime(now.Year, ((now.Month - 1) / 3 * 3) + 1, 1),new DateTime(now.Year, ((now.Month - 1) / 3 * 3) + 1, 1).AddMonths(3)),
                "ANNUAL" => (new DateTime(now.Year, 1, 1),new DateTime(now.Year + 1, 1, 1)),
                _ => (DateTime.MinValue,DateTime.MaxValue)
            };
        }

        private static string GetCircularStatus(Circular c) {
            if (c.IsBreached)
                return "BREACHED";

            return c.Status ?? "OPEN";
        }

        private static string MapPolicyStatusToFilter(PolicyStatus status) => status switch {
            PolicyStatus.ONHOLD => "ON-HOLD",
            PolicyStatus.NEEDREVIEW => "DUE",
            PolicyStatus.PENDINGBOARD => "PENDING-BOARD",
            PolicyStatus.PENDINGDEPARTMENT => "DEPT-REVIEW",
            PolicyStatus.UPTODATE => "UPTODATE",
            PolicyStatus.STANDARD => "NA",
            _ => null
        };

        private static string MapReturnStatusToFilter(ReportPeriod status) => status switch {
            ReportPeriod.DAILY => "DAILY",
            ReportPeriod.WEEKLY => "WEEKLY",
            ReportPeriod.MONTHLY => "MONTHLY",
            ReportPeriod.QUARTERLY => "QUARTERLY",
            ReportPeriod.BIANNUAL => "BIANNUAL",
            ReportPeriod.ANNUAL => "ANNUAL",
            ReportPeriod.BIENNIAL => "BIENNIAL",
            ReportPeriod.TRIENNIAL => "TRIENNIAL",
            ReportPeriod.PERIODIC => "PERIODIC",
            ReportPeriod.NA => "NA",
            ReportPeriod.ONEOFF => "ONE-OFF",
            _ => "ON OCCURRENCE"
        };

        private static bool BelongsToPeriod(ReturnSubmission s, ReportPeriod period) {
            var today = DateTime.Today;

            if (s.PeriodStart == default || s.PeriodEnd == default)
                return false;

            return period switch {
                ReportPeriod.DAILY =>
                    s.PeriodStart.Date == today,

                ReportPeriod.WEEKLY =>
                    s.PeriodStart.Date <= today &&
                    s.PeriodEnd.Date >= today &&
                    (s.PeriodEnd - s.PeriodStart).TotalDays <= 7,

                ReportPeriod.MONTHLY =>
                    s.PeriodStart.Year == today.Year &&
                    s.PeriodStart.Month == today.Month,

                ReportPeriod.QUARTERLY =>
                    s.PeriodStart.Year == today.Year &&
                    GetQuarter(s.PeriodStart) == GetQuarter(today),

                ReportPeriod.BIANNUAL =>
                    s.PeriodStart.Year == today.Year &&
                    GetHalf(s.PeriodStart) == GetHalf(today),

                ReportPeriod.ANNUAL =>
                    s.PeriodStart.Year == today.Year,

                ReportPeriod.BIENNIAL =>
                    s.PeriodStart.Year / 2 == today.Year / 2,

                ReportPeriod.TRIENNIAL =>
                    s.PeriodStart.Year / 3 == today.Year / 3,

                //..none time based periods
                ReportPeriod.ONEOFF or
                ReportPeriod.ONOCCURRENCE or
                ReportPeriod.PERIODIC =>
                    true,

                _ => false
            };
        }

        private static ReportPeriod MapFrequencyToReportPeriod(string frequencyName) {
            return frequencyName?.ToUpperInvariant() switch {
                "DAILY" => ReportPeriod.DAILY,
                "WEEKLY" => ReportPeriod.WEEKLY,
                "MONTHLY" => ReportPeriod.MONTHLY,
                "QUARTERLY" => ReportPeriod.QUARTERLY,
                "BIANNUAL" => ReportPeriod.BIANNUAL,
                "ANNUAL" => ReportPeriod.ANNUAL,
                "BIENNIAL" => ReportPeriod.BIENNIAL,
                "TRIENNIAL" => ReportPeriod.TRIENNIAL,
                "ONE-OFF" => ReportPeriod.ONEOFF,
                "ON OCCURRENCE" => ReportPeriod.ONOCCURRENCE,
                "PERIODIC" => ReportPeriod.PERIODIC,
                _ => ReportPeriod.NA
            };
        }

        private static int GetQuarter(DateTime date) 
            => (date.Month - 1) / 3 + 1;

        private static int GetHalf(DateTime date) 
            => date.Month <= 6 ? 1 : 2;

        private void LogError(IUnitOfWork uow, Exception ex) {
            var currentEx = ex.InnerException;
            while (currentEx != null) {
                Logger.LogActivity($"Service Inner Exception: {currentEx.Message}", "ERROR");
                currentEx = currentEx.InnerException;
            }

            // Get company ID efficiently
            var company = uow.CompanyRepository.GetAll(c => true, false);
            long companyId = company.FirstOrDefault()?.Id ?? 1L;

            // Get innermost exception
            var innermostException = ex;
            while (innermostException.InnerException != null)
                innermostException = innermostException.InnerException;

            var errorObj = new SystemError {
                ErrorMessage = innermostException.Message,
                ErrorSource = "REGULATORY-RETURN-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            uow.SystemErrorRespository.Insert(errorObj);
            uow.SaveChanges();
        }

        private async Task LogErrorAsync(IUnitOfWork uow, Exception ex) {
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
            Logger.LogActivity($"{ex.StackTrace}", "ERROR");

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            SystemError errorObj = new() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "REGULATORY-RETURN-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            //..save error object to the database
            _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
        }

        private static double CalculateCircularPercentage(int count, int total) {
            if (total == 0) return 0;
            return Math.Round((double)count / total * 100, 2);
        }

        #endregion
    }
}
