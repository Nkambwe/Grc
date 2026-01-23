using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Audits {

    public class AuditService : BaseService, IAuditService {
        public AuditService(IServiceLoggerFactory loggerFactory, 
                            IUnitOfWorkFactory uowFactory, 
                            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        #region Statistics

        public async Task<AuditSupportResponse> GetAuditSupportItemsAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve support items for audits", "INFO");

            try {

                AuditSupportResponse response = new() {
                    Types = new(),
                    Authorities = new(),
                    Audits = new(),
                    Responsibilities = new()
                };


                // get audit types
                var types = await uow.AuditTypeRepository.GetAllAsync(includeDeleted);

                //..get audits
                var audits = await uow.AuditRepository.GetAllAsync(includeDeleted);

                // get authorities
                var authorities = await uow.AuthoritiesRepository.GetAllAsync(includeDeleted);

                // get authorities
                var responsibilities = await uow.ResponsebilityRepository.GetAllAsync(includeDeleted, r => r.Department);

                //..authorities
                if (authorities != null && authorities.Count > 0) {
                    response.Authorities.AddRange(
                        from authority in authorities
                        select new RegulatoryAuthorityResponse {
                            Id = authority.Id,
                            AuthorityAlias = authority.AuthorityAlias,
                            AuthorityName = authority.AuthorityName,
                            IsDeleted = authority.IsDeleted,
                            CreatedOn = authority.CreatedOn,
                            UpdatedOn = authority.LastModifiedOn ?? DateTime.Now
                        });
                    Logger.LogActivity($"Authorities found: {authorities.Count}", "DEBUG");
                }

                //..responsibilities
                if (responsibilities != null && responsibilities.Count > 0) {
                    response.Responsibilities.AddRange(
                        from resp in responsibilities
                        select new ResponsibilityItemResponse {
                            Id = resp.Id,
                            ResponsibilityRole = resp.ContactPosition,
                            DepartmentName = resp.Department?.DepartmentName ?? string.Empty
                        });
                    Logger.LogActivity($"Responsibilities found: {authorities.Count}", "DEBUG");
                }

                //..audit types
                if (types != null && types.Count > 0) {
                    response.Types.AddRange(
                        from type in types
                        select new AuditMiniTypeResponse {
                            Id = type.Id,
                            TypeName = type.TypeName
                        });
                    Logger.LogActivity($"Audit types found: {types.Count}", "DEBUG");
                }

                //..audit
                if (audits != null && audits.Count > 0) {
                    response.Audits.AddRange(
                        from name in audits
                        select new MiniAuditResponse {
                            Id = name.Id,
                            AuditName = name.AuditName
                        });
                    Logger.LogActivity($"Audits found: {types.Count}", "DEBUG");
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve support items in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<AuditDashboardResponse> GetAuditDashboardStatisticsAsync(bool includeDeletes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve audit dashboard", "INFO");

            try {

                var result = new AuditDashboardResponse() {
                    Findings = new(),
                    Completions = new(),
                    BarChart = new()
                };

                //..benchmark
                var today = DateTime.Now;
                var currentYear = DateTime.Now.Year;

                //..get audit records
                var audits = await uow.AuditExceptionRepository.GetAllAsync( a => a.TargetDate.Year == currentYear ||
                                   (a.TargetDate.Year < currentYear && (a.Status == "OPEN" || a.Status == "PENDING")), includeDeletes);
                //..get audits
                var reports = await uow.AuditReportRepository.GetAllAsync(r => r.AuditedOn.Year == currentYear ||
                                     r.AuditedOn.Year < currentYear && r.Status == "OPEN" || r.Status == "PENDING",
                                     includeDeletes,
                                     a => a.Audit,
                                     a => a.Audit.AuditType,
                                     a => a.Audit.Authority,
                                     a => a.AuditExceptions);
                //..add exceptions
                var totals = audits.Count;
                var monthLess = audits.Count(a => a.TargetDate < today.AddMonths(1));
                var monthDue = audits.Count(a => a.TargetDate >= today.AddMonths(1) && a.TargetDate < today.AddMonths(2));
                var twoSixDue = audits.Count(a => a.TargetDate >= today.AddMonths(2) && a.TargetDate < today.AddMonths(6));
                var aboveSix = audits.Count(a => a.TargetDate >= today.AddMonths(6));

                var findings = new Dictionary<string, int> {
                    {"Total Exceptions", totals },
                    {"Due less than a Month", monthLess},
                    {"Due in a Month", monthDue},
                    {"Due 2 to 6 Months", twoSixDue },
                    {"Due above 6 months", aboveSix}
                };

                //..completion
                var closed = audits.Count(a => a.Status == "CLOSED");
                var allOpen = audits.Count(a => a.Status == "OPEN");
                var due = audits.Count(a => a.Status != "CLOSED" && a.TargetDate < today);
                var open = allOpen - due;
                var completions = new Dictionary<string, int> {
                    { "Closed", closed},
                    { "Open",  open},
                    { "Over Due",due}
                };

                var barChart = reports
                    .GroupBy(a => a.Audit.Authority.AuthorityAlias)
                    .ToDictionary(
                        g => g.Key,
                        g => new Dictionary<string, int>{
                            {"Closed", g.Count(a => a.Status == "CLOSED")},
                            {"Open", g.Count(a => a.Status == "OPEN") },
                            {"Over Due", g.Count(a => a.Status != "CLOSED" && a.AuditedOn < today.AddDays(10))}
                        }
                    );

                if (findings != null && findings.Count > 0) {
                    result.Findings = findings;
                }

                if (completions != null && completions.Count > 0) {
                    result.Completions = completions;
                }

                if (barChart != null && barChart.Count > 0) {
                    result.BarChart = barChart;
                }

                return result;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate audit statistics in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }

        }

        public async Task<AuditMiniReportResponse> GetAuditMiniStatisticsAsync(long recordId, bool includeDeleted) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve audit dashboard", "INFO");

            try {
                var today = DateTime.Now;
                var result = await uow.AuditReportRepository.GetAsync(
                                    r => r.Id == recordId, includeDeleted,
                                    a => a.Audit, a => a.Audit.AuditType, 
                                    a => a.Audit.Authority,a => a.AuditExceptions);

                AuditMiniReportResponse report = null;
                var exceptions = result.AuditExceptions;
                var completed = 0.0M;
                var outstanding = 0.0M;
                var closed = 0;
                var overdue = 0;
                var open = 0;
                var total = 0;
                if (exceptions != null && exceptions.Count > 0 ) {

                    //..calculate percentage completion
                    total = exceptions.Count;
                    var c_count = exceptions.Count(e => e.Status == "CLOSED");
                    var o_count = exceptions.Count(e => e.Status == "OPEN");

                    completed = c_count / total * 100;
                    outstanding = o_count / total * 100;

                    //..clalculate status
                    var allOpen = exceptions.Count(e => e.Status == "OPEN");
                    closed = exceptions.Count(e => e.Status == "CLOSED");
                    overdue = exceptions.Count(e => e.Status == "OPEN" && e.TargetDate < today);
                    open = allOpen - overdue;
                }

                if (result != null) {
                    report = new() {
                        Id = 1,
                        Reference = result.Reference ?? string.Empty,
                        ReportName = result.ReportName ?? string.Empty,
                        Status = result.AuditedOn < today.AddDays(-10) ? "DUE" : result.Status ?? string.Empty,
                        Total = total,
                        Closed = closed,
                        Open = open,
                        Overdue = overdue,
                        AuditedOn = result.AuditedOn,
                        Completed = completed,
                        Outstanding = outstanding,
                        Exceptions = exceptions != null && exceptions.Count > 0 ?
                        exceptions.Select(exc => new AuditExceptionResponse() {
                            Id = exc.Id,
                            Finding = exc.AuditFinding,
                            ProposedAction = exc.ProposedAction,
                            Notes = exc.ExceptionNotes,
                            TargetDate = exc.TargetDate,
                            RiskStatement = exc.RiskAssessment,
                            RiskRating = exc.RiskRating,
                            Responsible = exc.Responseability?.ContactPosition ?? string.Empty,
                            Executioner = exc.Executioner ?? string.Empty,
                            Status = exc.TargetDate < today.AddDays(-10) ? "DUE" : exc.Status ?? string.Empty,
                        }).ToList():
                        new List<AuditExceptionResponse>()
                    };
                }

                return report;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit report in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<AuditExtensionStatistics> GetPeriodStatisticsAsync(string period, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve audit period exceptions dashboard", "INFO");

            try {
                var statuses = new Dictionary<string, int>();
                var result = new AuditExtensionStatistics() {
                    Statuses = new(),
                    Reports = new()
                };

                var today = DateTime.UtcNow;
                var currentYear = DateTime.UtcNow.Year;

                var reports = await uow.AuditReportRepository.GetAllAsync(
                    r => r.AuditedOn.Year == currentYear || (r.AuditedOn.Year < currentYear && (r.Status == "OPEN" || r.Status == "PENDING")),
                    includeDeleted,
                    a => a.Audit, a => a.Audit.AuditType, a => a.Audit.Authority, a => a.AuditExceptions);

                //..statuses
                Func<AuditException, bool> periodFilter = GetPeriodFilter(period);
                var filteredExceptions = reports.SelectMany(r => r.AuditExceptions).Where(periodFilter).ToList();
                if (filteredExceptions.Count > 0) {
                    var total = filteredExceptions.Count;
                    var allOpen = filteredExceptions.Count(e => e.Status == "OPEN");
                    var closed = filteredExceptions.Count(e => e.Status == "CLOSED");
                    var overdue = filteredExceptions.Count(e => e.Status == "OPEN" && e.TargetDate < today);
                    var open = allOpen - overdue;

                    statuses = new Dictionary<string, int> {
                        { "Total", total },
                        { "Open", open },
                        { "Close", closed },
                        { "Due", overdue }
                    };
                }

                if (statuses != null && statuses.Any()) {
                    result.Statuses = statuses;
                } else {
                    statuses = new() { { "Total", 0 }, { "Open", 0 }, { "Close", 0 }, { "Due", 0 } };
                }

                //..add reports
                var miniReports = reports.Select(r => {
                    var exceptions = r.AuditExceptions.Where(periodFilter).ToList();
                    var total = exceptions == null ? 0 : exceptions.Count;
                    if (total == 0)
                        return null;

                    var closed = exceptions.Count(e => e.Status == "CLOSED");
                    var allOpen = exceptions.Count(e => e.Status == "OPEN");
                    var overdue = exceptions.Count(e => e.Status == "OPEN" && e.TargetDate < today.AddDays(-10));
                    var open = allOpen - overdue;
                    return new AuditMiniReportResponse {
                        Id = r.Id,
                        Reference = r.Reference,
                        ReportName = r.ReportName,
                        Total = total,
                        Closed = closed,
                        Open = open,
                        Overdue = overdue,
                        Status = r.AuditedOn < today.AddDays(-10) ? "DUE": r.Status,
                        AuditedOn = r.AuditedOn,
                        Completed = Math.Round((decimal)closed / total * 100, 2),
                        Outstanding = Math.Round((decimal)open / total * 100, 2)
                    };
                }).Where(r => r != null).OrderByDescending(r => r.AuditedOn).ToList();
            
                if (miniReports != null && miniReports.Any()) {
                    result.Reports = miniReports;
                    result.Statuses = statuses;
                } else {
                   result = new AuditExtensionStatistics() {
                       Statuses = statuses,
                       Reports = new()
                   };
                }

                return result;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate audit statistics in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<List<AuditMiniReportResponse>> GetMiniPeriodStatisticsAsync(string period, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve audit dashboard", "INFO");

            try {
                var today = DateTime.UtcNow;
                var currentYear = DateTime.UtcNow.Year;
                Func<AuditException, bool> periodFilter = GetPeriodFilter(period);
                var reports = await uow.AuditReportRepository.GetAllAsync(
                    r => r.AuditedOn.Year == currentYear || (r.AuditedOn.Year < currentYear && (r.Status == "OPEN" || r.Status == "PENDING")),
                    includeDeleted, a => a.Audit, a => a.Audit.AuditType, a => a.Audit.Authority, a => a.AuditExceptions);

                var miniReports = reports.Select(r => {
                    var exceptions = r.AuditExceptions.Where(periodFilter).ToList();
                    var total = exceptions == null ? 0 : exceptions.Count;
                    if (total == 0)
                        return null;

                    var closed = exceptions.Count(e => e.Status == "CLOSED");
                    var allOpen = exceptions.Count(e => e.Status == "OPEN");
                    var overdue = exceptions.Count(e => e.Status == "OPEN" && e.TargetDate < today.AddDays(-10));
                    var open = allOpen - overdue;
                    return new AuditMiniReportResponse {
                        Id = r.Id,
                        Reference = r.Reference,
                        ReportName = r.ReportName,
                        Total = total,
                        Closed = closed,
                        Open = open,
                        Overdue = overdue,
                        Status = r.AuditedOn < today.AddDays(-10) ? "DUE" : r.Status,
                        AuditedOn = r.AuditedOn,
                        Completed = Math.Round((decimal)closed / total * 100, 2),
                        Outstanding = Math.Round((decimal)open / total * 100, 2)
                    };
                }).Where(r => r != null).OrderByDescending(r => r.AuditedOn).ToList();

                //..return
                return miniReports != null ? miniReports : new List<AuditMiniReportResponse>();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve period statistics in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        #endregion

        #region Queries
        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audits in the database", "INFO");

            try
            {
                return uow.AuditRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public int Count(Expression<Func<Audit, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audits in the database", "INFO");

            try
            {
                return uow.AuditRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audits in the database", "INFO");

            try
            {
                return await uow.AuditRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audits in the database", "INFO");

            try
            {
                return await uow.AuditRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<Audit, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audits in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<Audit, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audits in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public bool Exists(Expression<Func<Audit, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an audit exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.AuditRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Audit, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an audits exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Audit, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch audits if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.AuditRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audits in the database: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public Audit Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit with ID '{id}'", "INFO");

            try
            {
                return uow.AuditRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public Audit Get(Expression<Func<Audit, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.AuditRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public Audit Get(Expression<Func<Audit, bool>> predicate, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audits that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.AuditRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public IQueryable<Audit> GetAll(bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit", "INFO");

            try
            {
                return uow.AuditRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public IList<Audit> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit", "INFO");

            try
            {
                return uow.AuditRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public IList<Audit> GetAll(Expression<Func<Audit, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.AuditRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<IList<Audit>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit", "INFO");

            try
            {
                return await uow.AuditRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<IList<Audit>> GetAllAsync(Expression<Func<Audit, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audits that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<IList<Audit>> GetAllAsync(Expression<Func<Audit, bool>> predicate, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audits that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<IList<Audit>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all audits", "INFO");

            try
            {
                return await uow.AuditRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<Audit> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit with ID '{id}'", "INFO");

            try
            {
                return await uow.AuditRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<Audit> GetAsync(Expression<Func<Audit, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<Audit> GetAsync(Expression<Func<Audit, bool>> predicate, bool includeDeleted = false, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.AuditRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<IList<Audit>> GetTopAsync(Expression<Func<Audit, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} audits that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.AuditRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit : {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public bool Insert(AuditRequest request, string username)
        {
            using var uow = UowFactory.Create();
            try {
                //..map audit request to audit entity
                var audit = new Audit() { 
                    AuditName = request.AuditName,
                    Notes = request.Notes,
                    AuditTypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    CreatedBy = username,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = username,
                    LastModifiedOn = DateTime.Now
                };

                //..log the audit data being saved
                var auditJson = JsonSerializer.Serialize(audit, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");

                var added = uow.AuditRepository.Insert(audit);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<bool> InsertAsync(AuditRequest request, string username)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map audit request to audit entity
                var audit = new Audit() {
                    AuditName = request.AuditName,
                    Notes = request.Notes,
                    AuditTypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    CreatedBy = username,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = username,
                    LastModifiedOn = DateTime.Now
                };

                //..log the audit data being saved
                var auditJson = JsonSerializer.Serialize(audit, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");

                var added = await uow.AuditRepository.InsertAsync(audit);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public bool Update(AuditRequest request, string username, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update audit", "INFO");

            try
            {
                var audit = uow.AuditRepository.Get(a => a.Id == request.Id);
                if (audit != null)
                {
                    //..update audit record
                    audit.AuditName = (request.AuditName ?? string.Empty).Trim();
                    audit.Notes = (request.Notes ?? string.Empty).Trim();
                    audit.AuthorityId = request.AuthorityId;
                    audit.AuditTypeId = request.TypeId;
                    audit.IsDeleted = request.IsDeleted;
                    audit.LastModifiedOn = DateTime.Now;
                    audit.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = uow.AuditRepository.Update(audit, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Audit state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update audit record: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AuditRequest request, string username, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update audit", "INFO");

            try
            {
                var audit = await uow.AuditRepository.GetAsync(a => a.Id == request.Id);
                if (audit != null)
                {
                    //..update audit record
                    audit.AuditName = (request.AuditName ?? string.Empty).Trim();
                    audit.Notes = (request.Notes ?? string.Empty).Trim();
                    audit.AuthorityId = request.AuthorityId;
                    audit.AuditTypeId = request.TypeId;
                    audit.IsDeleted = request.IsDeleted;
                    audit.LastModifiedOn = DateTime.Now;
                    audit.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.AuditRepository.UpdateAsync(audit, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Audit state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update audit record: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try
            {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");

                var audit = uow.AuditRepository.Get(t => t.Id == request.RecordId);
                if (audit != null)
                {
                    //..mark as delete this audit
                    _ = uow.AuditRepository.Delete(audit, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Audit : {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");

                var exception = await uow.AuditRepository.GetAsync(t => t.Id == request.RecordId);
                if (exception != null)
                {
                    //..mark as delete this audit
                    _ = await uow.AuditRepository.DeleteAsync(exception, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(exception).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Audit : {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var company = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requesItems, bool markAsDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                var audits = await uow.AuditRepository.GetAllAsync(e => requesItems.Contains(e.Id));
                if (audits.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.AuditRepository.DeleteAllAsync(audits, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Audit: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(AuditRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map audit request to audit entity
                var audits = requestItems.Select(Mapper.Map<AuditRequest, Audit>).ToArray();
                var auditJson = JsonSerializer.Serialize(audits, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");
                return await uow.AuditRepository.BulkyInsertAsync(audits);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(AuditRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map audit request to audit entity
                var audit = requestItems.Select(Mapper.Map<AuditRequest, Audit>).ToArray();
                var auditJson = JsonSerializer.Serialize(audit, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");
                return await uow.AuditRepository.BulkyUpdateAsync(audit);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(AuditRequest[] requestItems, params Expression<Func<Audit, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map audit request to audit entity
                var audits = requestItems.Select(Mapper.Map<AuditRequest, Audit>).ToArray();
                var auditJson = JsonSerializer.Serialize(audits, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit data: {auditJson}", "DEBUG");
                return await uow.AuditRepository.BulkyUpdateAsync(audits, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audits : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audits", "INFO");

            try
            {
                return await uow.AuditRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(errorObj);
                throw;
            }
        }

        public async Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Audit, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audits ", "INFO");

            try
            {
                return await uow.AuditRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Audit, bool>> where = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit", "INFO");

            try
            {
                return await uow.AuditRepository.PageAllAsync(page, size, includeDeleted, where);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<PagedResult<Audit>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Audit, bool>> where = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audits", "INFO");

            try
            {
                return await uow.AuditRepository.PageAllAsync(token, page, size, where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audits : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<PagedResult<Audit>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Audit, bool>> predicate = null, params Expression<Func<Audit, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audits", "INFO");

            try {
                return await uow.AuditRepository.PageAllAsync(page, size, includeDeleted, predicate, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audits : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        #endregion

        #region private methods

        private static Func<AuditException, bool> GetPeriodFilter(string period) {
            var today = DateTime.Now;
            return  period switch {
                "MONTHLESS" => e => e.TargetDate < today.AddMonths(1),
                "ONEMONTH" => e => e.TargetDate >= today.AddMonths(1) && e.TargetDate < today.AddMonths(2),
                "TWOSIXMONTHS" => e => e.TargetDate >= today.AddMonths(2) && e.TargetDate < today.AddMonths(6),
                "ABOVESIXMONTHS" => e => e.TargetDate >= today.AddMonths(6),
                _ => throw new NotSupportedException("Reporting period not supported for exceptions")
            };
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

        #endregion

    }
}
