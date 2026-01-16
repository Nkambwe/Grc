using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.System;
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

        public async Task<AuditDashboardResponse> GetAuditDashboardStatisticsAsync(bool includeDeletes) {
            return await Task.FromResult(new AuditDashboardResponse() {
                Findings = new() {
                            {"Total", 80 },
                            {"< 1 Month", 10 },
                            {"1 to 2 Months", 50 },
                            {"> 3 Months", 20 }
                        },
                Completions = new() {
                            {"Closed", 70 },
                            {"Open", 60 },
                            {"Over Due", 30 }
                        },
                BarChart = new() {
                            {"BOU", new Dictionary<string, int>(){
                                {"Closed", 20 },
                                {"Open", 30 },
                                {"Over Due", 10 }
                            } },
                            {"KPMG",  new Dictionary<string, int>(){
                                {"Closed", 20 },
                                {"Open", 40 },
                                {"Over Due",15 }
                            } },
                            {"IRA",  new Dictionary<string, int>(){
                                {"Closed", 30 },
                                {"Open", 10 },
                                {"Over Due", 30 }
                            } },
                            {"ER",  new Dictionary<string, int>(){
                                {"Closed", 8 },
                                {"Open", 12 },
                                {"Over Due", 5 }
                            }}
                        }
            });

        }

        public async Task<AuditMiniReportResponse> GetAuditMiniStatisticsAsync(long recordId, bool includeDeleted) {
            return await Task.FromResult(new AuditMiniReportResponse() {
                Id = 1,
                Reference = "BOU OUT/1",
                ReportName = "Sample BOU Report",
                Status = "Pending",
                Total = 30,
                Closed = 10,
                Open = 12,
                Overdue = 8,
                AuditedOn = new DateTime(2024, 2, 15),
                Completed = 80.0M,
                Outstanding = 20.0M,
                Exceptions = new List<AuditExceptionResponse> {
                        new() {
                            Id = 1,
                            Finding = "Sample finding",
                            ProposedAction = "Sample proposed fix",
                            Notes = "Sample proposed notes",
                            TargetDate = new DateTime(2026,4,12),
                            RiskStatement = "CRITICAL",
                            RiskRating = 70,
                            Responsible = "MD's Office",
                            Executioner = "Seccretary",
                            Status = "CLOSED",
                        },
                        new() {
                            Id = 1,
                            Finding = "Sample finding 2",
                            ProposedAction = "Sample proposed fix 2",
                            Notes = "Sample proposed notes 2",
                            TargetDate = new DateTime(2026,6,12),
                            RiskStatement = "MEDIUM",
                            RiskRating = 70,
                            Responsible = "MD's Office",
                            Executioner = "Seccretary",
                            Status = "OPEN",
                        }
                        ,
                        new() {
                            Id = 1,
                            Finding = "Sample finding 3",
                            ProposedAction = "Sample proposed fix 3",
                            Notes = "Sample proposed notes 3",
                            TargetDate = new DateTime(2025,12,23),
                            RiskStatement = "LOW",
                            RiskRating = 70,
                            Responsible = "MD's Office",
                            Executioner = "Seccretary",
                            Status = "OVER DUE",
                        }
                    }
            });
        }

        public async Task<AuditExtensionStatistics> GetPeriodStatisticsAsync(string period, bool includeDeleted) {
            var statuses = new Dictionary<string, int>() { { "Total", 80 }, { "Open", 10 }, { "Close", 50 }, { "Due", 20 } };
            return await Task.FromResult(new AuditExtensionStatistics() {
                Statuses = statuses,
                Reports = new List<AuditMiniReportResponse>() {
                        new () {
                            Id =  1,
                            Reference = "BOU OUT/1",
                            ReportName = "Sample BOU Report",
                            Total = 30,
                            Closed = 10,
                            Open = 12,
                            Overdue = 8,
                            AuditedOn = new DateTime(2024,2,15),
                            Completed = 80.0M,
                            Outstanding = 20.0M
                        },
                        new () {
                            Id =  3,
                            Reference = "KMGT EXT/1",
                            ReportName = "Sample KMGT Report",
                            Total = 20,
                            Closed = 8,
                            Open = 10,
                            Overdue = 2,
                            AuditedOn = new DateTime(2024,6,15),
                            Completed = 40.0M,
                            Outstanding = 20.0M
                        },
                        new () {
                            Id =  3,
                            Reference = "IR EXT/1",
                            ReportName = "Sample IR Report",
                            Total = 50,
                            Closed = 50,
                            Open = 0,
                            Overdue = 0,
                            AuditedOn = new DateTime(2024,3,15),
                            Completed = 100.0M,
                            Outstanding = 0.0M
                        }
                }
            });
        }

        public async Task<List<AuditMiniReportResponse>> GetMiniPeriodStatisticsAsync(string period, bool includeDeleted) {
            return await Task.FromResult(new List<AuditMiniReportResponse>() {
                new () {
                    Id =  1,
                    Reference = "BOU OUT/1",
                    ReportName = "Sample BOU Report",
                    Total = 30,
                    Closed = 10,
                    Open = 12,
                    Overdue = 8,
                    AuditedOn = new DateTime(2024,2,15),
                    Completed = 80.0M,
                    Outstanding = 20.0M
                },
                new () {
                    Id =  2,
                    Reference = "KMGT EXT/1",
                    ReportName = "Sample KMGT Report",
                    Total = 20,
                    Closed = 8,
                    Open = 10,
                    Overdue = 2,
                    AuditedOn = new DateTime(2024,6,15),
                    Completed = 40.0M,
                    Outstanding = 20.0M
                },
                new () {
                    Id =  3,
                    Reference = "IR EXT/1",
                    ReportName = "Sample IR Report",
                    Total = 50,
                    Closed = 50,
                    Open = 0,
                    Overdue = 0,
                    AuditedOn = new DateTime(2024,3,15),
                    Completed = 100.0M,
                    Outstanding = 0.0M
                }
            });
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
                    _ = uow.AuditRepository.Delete(audit, request.markAsDeleted);

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
                    _ = await uow.AuditRepository.DeleteAsync(exception, request.markAsDeleted);

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

        #endregion

    }
}
