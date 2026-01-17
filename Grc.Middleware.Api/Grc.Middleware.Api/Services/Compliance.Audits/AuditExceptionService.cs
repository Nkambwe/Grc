using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;
using RTools_NTS.Util;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Audits {

    public class AuditExceptionService : BaseService, IAuditExceptionService {
        public AuditExceptionService(IServiceLoggerFactory loggerFactory, 
                                     IUnitOfWorkFactory uowFactory, 
                                     IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return uow.AuditExceptionRepository.Count(); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public int Count(Expression<Func<AuditException, bool>> predicate) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return uow.AuditExceptionRepository.Count(predicate); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<int> CountAsync(CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return await uow.AuditExceptionRepository.CountAsync(cancellationToken); 
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database", "INFO");

            try {
                return await uow.AuditExceptionRepository.CountAsync(excludeDeleted, cancellationToken);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, CancellationToken cancellationToken = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of audit exceptions in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public bool Exists(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an audit exceptions exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<bool> ExistsAsync(Expression<Func<AuditException, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an audit exceptions exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<AuditException, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch audit exceptions if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for audit exceptions in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public AuditException Get(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions with ID '{id}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fits predicate >> '{where}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Get(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public AuditException Get(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fits predicate >> '{where}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.Get(where, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public IQueryable<AuditException> GetAll(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions", "INFO");

            try
            {
                return uow.AuditExceptionRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public IList<AuditException> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions", "INFO");

            try
            {
                return uow.AuditExceptionRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public IList<AuditException> GetAll(Expression<Func<AuditException, bool>> where, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return uow.AuditExceptionRepository.GetAll(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<IList<AuditException>> GetAllAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(where, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<IList<AuditException>> GetAllAsync(bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<AuditException> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions with ID '{id}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAsync(where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<AuditException> GetAsync(Expression<Func<AuditException, bool>> where, bool includeDeleted = false, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get audit exceptions that fit predicate '{where}'", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetAsync(where, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<IList<AuditException>> GetTopAsync(Expression<Func<AuditException, bool>> where, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} audit exceptions that fit predicate >> {where}", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.GetTopAsync(where, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorMessage = innerEx != null ? innerEx.Message: ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public bool Insert(AuditExceptionRequest request, string username) {
            using var uow = UowFactory.Create();
            try
            {
                //..map exception request to exception entity
                var exception = new AuditException() {
                    AuditFinding = request.Findings,
                    RemediationPlan = request.ProposedAction,
                    CorrectiveAction = request.CorrectiveAction,
                    ExceptionNotes = request.Notes,
                    TargetDate = request.TargetDate,
                    RiskAssessment = request.RiskLevel,
                    RiskRating = request.RiskRate,
                    ResponsabilityId = request.ResponsibileId,
                    Executioner = request.Executioner,
                    Status = request.Status,
                    AuditReportId = request.ReportId,
                    IsDeleted = request.IsDeleted,
                    LastModifiedOn = DateTime.Now,
                    LastModifiedBy = $"{username}"
                };

                //..log the audit exception data being saved
                var exJson = JsonSerializer.Serialize(exception, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit exception data: {exJson}", "DEBUG");

                var added = uow.AuditExceptionRepository.Insert(exception);
                if (added){
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(exception).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<bool> InsertAsync(AuditExceptionRequest request, string username) {
            using var uow = UowFactory.Create();
            try {
                var exception = new AuditException() {
                    AuditFinding = request.Findings,
                    RemediationPlan = request.ProposedAction,
                    CorrectiveAction = request.CorrectiveAction,
                    ExceptionNotes = request.Notes,
                    TargetDate = request.TargetDate,
                    RiskAssessment = request.RiskLevel,
                    RiskRating = request.RiskRate,
                    ResponsabilityId = request.ResponsibileId,
                    Executioner = request.Executioner,
                    Status = request.Status,
                    AuditReportId = request.ReportId,
                    IsDeleted = request.IsDeleted,
                    LastModifiedOn = DateTime.Now,
                    LastModifiedBy = $"{username}"
                };

                //..log the object data being saved
                var branchJson = JsonSerializer.Serialize(exception, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit Exception data: {branchJson}", "DEBUG");
                await uow.AuditExceptionRepository.InsertAsync(exception);

                //..check audit exception state
                var entityState = ((UnitOfWork)uow).Context.Entry(exception).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save audit exception: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public bool Update(AuditExceptionRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"ACTION >> {request.Action}", "INFO");

            try {
                var audit = uow.AuditExceptionRepository.Get(a => a.Id == request.Id);
                if (audit != null) {
                    //..update audit exception record
                    audit.AuditFinding = request.Findings;
                    audit.RemediationPlan = request.ProposedAction;
                    audit.CorrectiveAction = request.CorrectiveAction;
                    audit.ExceptionNotes = request.Notes;
                    audit.TargetDate = request.TargetDate;
                    audit.RiskAssessment = request.RiskLevel;
                    audit.RiskRating = request.RiskRate;
                    audit.ResponsabilityId = request.ResponsibileId;
                    audit.Executioner = request.Executioner;
                    audit.Status = request.Status;
                    audit.AuditReportId = request.ReportId;
                    audit.IsDeleted = request.IsDeleted;
                    audit.LastModifiedOn = DateTime.Now;
                    audit.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = uow.AuditExceptionRepository.Update(audit, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update audit exception record: {ex.Message}", "ERROR");

                //..log inner exceptions here too
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<bool> UpdateAsync(AuditExceptionRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"ACTION >> {request.Action}", "INFO");

            try {

                var audit = await uow.AuditExceptionRepository.GetAsync(a => a.Id == request.Id);
                if (audit != null) {
                    //..update audit exception record
                    audit.AuditFinding = request.Findings;
                    audit.RemediationPlan = request.ProposedAction;
                    audit.CorrectiveAction = request.CorrectiveAction;
                    audit.ExceptionNotes = request.Notes;
                    audit.TargetDate = request.TargetDate;
                    audit.RiskAssessment = request.RiskLevel;
                    audit.RiskRating = request.RiskRate;
                    audit.ResponsabilityId = request.ResponsibileId;
                    audit.Executioner = request.Executioner;
                    audit.Status = request.Status;
                    audit.AuditReportId = request.ReportId;
                    audit.IsDeleted = request.IsDeleted;
                    audit.LastModifiedOn = DateTime.Now;
                    audit.LastModifiedBy = $"{username}";

                    //..check entity state
                        _ = await uow.AuditExceptionRepository.UpdateAsync(audit, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(audit).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update audit exception record: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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
            try {
                var exJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit exception data: {exJson}", "DEBUG");

                var exception = uow.AuditExceptionRepository.Get(t => t.Id == request.RecordId);
                if (exception != null) {
                    //..mark as delete this audit exception
                   _ = uow.AuditExceptionRepository.Delete(exception, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(exception).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Audit exception: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try
            {
                var exJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit exception data: {exJson}", "DEBUG");

                var exception = await uow.AuditExceptionRepository.GetAsync(t => t.Id == request.RecordId, false, ex=>ex.AuditTasks);
                if (exception != null)
                {
                    //..mark as delete this audit exception
                    _ = await uow.AuditExceptionRepository.DeleteAsync(exception, request.markAsDeleted);

                    if (exception.AuditTasks != null) {
                        foreach (var ex in exception.AuditTasks) {
                            ex.IsDeleted = true;
                            ex.LastModifiedOn = DateTime.UtcNow;
                            uow.Context.Entry(ex).State = EntityState.Modified;
                        }
                    }

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
                Logger.LogActivity($"Failed to delete Audit exception: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                var conpany = (await uow.CompanyRepository.GetAllAsync(false)).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new()
                {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requesItems, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();
            try {
                var exceptions = await uow.AuditExceptionRepository.GetAllAsync(e => requesItems.Contains(e.Id));
                if (exceptions.Count == 0)  {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.AuditExceptionRepository.DeleteAllAsync(exceptions, markAsDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Audit exceptions: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(AuditExceptionRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try
            {
                //..map exception request to exception entity
                var exceptions = requestItems.Select(Mapper.Map<AuditExceptionRequest, AuditException>).ToArray();
                var exJson = JsonSerializer.Serialize(exceptions, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit exception data: {exJson}", "DEBUG");
                return await uow.AuditExceptionRepository.BulkyInsertAsync(exceptions); 
            }
            catch (Exception ex) {
                Logger.LogActivity($"Failed to save audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(AuditExceptionRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try
            {
                //..map exception request to exception entity
                var exceptions = requestItems.Select(Mapper.Map<AuditExceptionRequest, AuditException>).ToArray();
                var exJson = JsonSerializer.Serialize(exceptions, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit exception data: {exJson}", "DEBUG");
                return await uow.AuditExceptionRepository.BulkyUpdateAsync(exceptions);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(AuditExceptionRequest[] requestItems, params Expression<Func<AuditException, object>>[] propertySelectors) {
            using var uow = UowFactory.Create();
            try
            {
                //..map exception request to exception entity
                var exceptions = requestItems.Select(Mapper.Map<AuditExceptionRequest, AuditException>).ToArray();
                var exJson = JsonSerializer.Serialize(exceptions, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Audit exception data: {exJson}", "DEBUG");
                return await uow.AuditExceptionRepository.BulkyUpdateAsync(exceptions, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save audit exception: {ex.Message}", "ERROR");
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
                    ErrorMessage = innerEx != null ? innerEx.Message: ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var conpany = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = conpany != null ? conpany.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<AuditException, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> where = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit exception", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.PageAllAsync(page, size, includeDeleted, where);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<PagedResult<AuditException>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<AuditException, bool>> where = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit exceptions", "INFO");

            try
            {
                return await uow.AuditExceptionRepository.PageAllAsync(token, page, size, where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<PagedResult<AuditException>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<AuditException, bool>> predicate = null, params Expression<Func<AuditException, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged audit exceptions", "INFO");

            try {
                return await uow.AuditExceptionRepository.PageAllAsync(page, size, includeDeleted, predicate, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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

        public async Task<PagedResult<AuditExceptionResponse>> PageLookupAsync<AuditExceptionResponse>(int page, int size, bool includeDeleted, Expression<Func<AuditException, AuditExceptionResponse>> selector) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Audit Exceptions", "INFO");

            try {
                return await uow.AuditExceptionRepository.PageLookupAsync(page, size, includeDeleted, selector);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve audit exception: {ex.Message}", "ERROR");
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
                    ErrorSource = "AUDIT-EXCEPTION-SERVICE",
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
    }
}
