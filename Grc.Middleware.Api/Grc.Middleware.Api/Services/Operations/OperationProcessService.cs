using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Operations {

    public class OperationProcessService : BaseService, IOperationProcessService
    {
        public OperationProcessService(IServiceLoggerFactory loggerFactory, 
                                       IUnitOfWorkFactory uowFactory, 
                                       IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }
        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database", "INFO");

            try
            {
                return uow.OperationProcessRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public int Count(Expression<Func<OperationProcess, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database", "INFO");

            try
            {
                return uow.OperationProcessRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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
            Logger.LogActivity($"Count number of Processes in the database", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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
            Logger.LogActivity($"Count number of  Processes in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public bool Exists(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Processes exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<bool> ExistsAsync(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Processes exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<OperationProcess, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Processes if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public OperationProcess Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get  Operation Process with ID '{id}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public OperationProcess Get(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get  Operation Process that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public OperationProcess Get(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Processes that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public IQueryable<OperationProcess> GetAll(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes", "INFO");

            try
            {
                return uow.OperationProcessRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public IList<OperationProcess> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes", "INFO");

            try
            {
                return uow.OperationProcessRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public IList<OperationProcess> GetAll(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<OperationProcess> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Operation Process with ID '{id}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Operation Process that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Operation Process that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<IList<OperationProcess>> GetTopAsync(Expression<Func<OperationProcess, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Processes that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public bool Insert(ProcessRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Operation Process request to Operation Process entity
                var process = Mapper.Map<ProcessRequest, OperationProcess>(request);

                //..log the Operation Process data being saved
                var processJson = JsonSerializer.Serialize(process, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var added = uow.OperationProcessRepository.Insert(process);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Operation Process : {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<bool> InsertAsync(ProcessRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Operation Process request to Operation Process entity
                var process = Mapper.Map<ProcessRequest, OperationProcess>(request);

                //..log the Operation Process data being saved
                var processJson = JsonSerializer.Serialize(process, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var added = await uow.OperationProcessRepository.InsertAsync(process);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Operation Process : {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company != null ? company.Id : 1;
                SystemError errorObj = new() {
                    ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public bool Update(ProcessRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Operation Process request", "INFO");

            try
            {
                var process = uow.OperationProcessRepository.Get(a => a.Id == request.Id);
                if (process != null)
                {
                    //..update  Operation Process record
                    process.ProcessName = (request.ProcessName ?? string.Empty).Trim();
                    process.Description = (request.Description ?? string.Empty).Trim();
                    process.CurrentVersion = (request.CurrentVersion ?? string.Empty).Trim();
                    process.FileName = (request.FilePath ?? string.Empty).Trim();
                    process.EffectiveDate = request.EffectiveDate;
                    process.LastUpdated = request.LastUpdated;
                    process.OriginalOnFile = request.OriginalOnFile;
                    process.ProcessStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    process.TypeId = request.TypeId;
                    process.UnitId = request.UnitId;
                    process.OwnerId = request.OwnerId;
                    process.ResponsibilityId = request.ResponsibilityId;
                    process.ReasonOnhold = (request.OnholdReason ?? string.Empty).Trim();
                    process.ApprovalComment = (request.ApprovalComment ?? string.Empty).Trim();
                    process.Comments = (request.Comments ?? string.Empty).Trim();
                    process.IsDeleted = request.IsDeleted;
                    process.LastModifiedOn = DateTime.Now;
                    process.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.OperationProcessRepository.Update(process, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Operation Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update  OperationProcess record: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<bool> UpdateAsync(ProcessRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Operation Process", "INFO");

            try
            {
                var process = await uow.OperationProcessRepository.GetAsync(a => a.Id == request.Id);
                if (process != null)
                {
                    //..update  Operation Process record
                    process.ProcessName = (request.ProcessName ?? string.Empty).Trim();
                    process.Description = (request.Description ?? string.Empty).Trim();
                    process.CurrentVersion = (request.CurrentVersion ?? string.Empty).Trim();
                    process.FileName = (request.FilePath ?? string.Empty).Trim();
                    process.EffectiveDate = request.EffectiveDate;
                    process.LastUpdated = request.LastUpdated;
                    process.OriginalOnFile = request.OriginalOnFile;
                    process.ProcessStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    process.TypeId = request.TypeId;
                    process.UnitId = request.UnitId;
                    process.OwnerId = request.OwnerId;
                    process.ResponsibilityId = request.ResponsibilityId;
                    process.ReasonOnhold = (request.OnholdReason ?? string.Empty).Trim();
                    process.ApprovalComment = (request.ApprovalComment ?? string.Empty).Trim();
                    process.Comments = (request.Comments ?? string.Empty).Trim();
                    process.IsDeleted = request.IsDeleted;
                    process.LastModifiedOn = DateTime.Now;
                    process.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.OperationProcessRepository.UpdateAsync(process, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Regulatory Operation Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Operation Process record: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public bool Delete(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var processJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var process = uow.OperationProcessRepository.Get(t => t.Id == request.RecordId);
                if (process != null)
                {
                    //..mark as delete this OperationProcess
                    _ = uow.OperationProcessRepository.Delete(process, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Operation Process : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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
                var processJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var responsebilitytask = await uow.OperationProcessRepository.GetAsync(t => t.Id == request.RecordId);
                if (responsebilitytask != null)
                {
                    //..mark as delete this OperationProcess
                    _ = await uow.OperationProcessRepository.DeleteAsync(responsebilitytask, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(responsebilitytask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete  OperationProcess : {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                var processes = await uow.OperationProcessRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (processes.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.OperationProcessRepository.DeleteAllAsync(processes, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Processes: {ex.Message}", "ERROR");

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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }

        }

        public async Task<bool> BulkyInsertAsync(ProcessRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Processes to Processes entity
                var processes = requestItems.Select(Mapper.Map<ProcessRequest, OperationProcess>).ToArray();
                var processJson = JsonSerializer.Serialize(processes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Processes data: {processJson}", "DEBUG");
                return await uow.OperationProcessRepository.BulkyInsertAsync(processes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Processes : {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(ProcessRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Processes request to Processes entity
                var processes = requestItems.Select(Mapper.Map<ProcessRequest, OperationProcess>).ToArray();
                var processJson = JsonSerializer.Serialize(processes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Processes data: {processJson}", "DEBUG");
                return await uow.OperationProcessRepository.BulkyUpdateAsync(processes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Processes : {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(ProcessRequest[] requestItems, params Expression<Func<OperationProcess, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Processes request to Processes entity
                var process = requestItems.Select(Mapper.Map<ProcessRequest, OperationProcess>).ToArray();
                var processJson = JsonSerializer.Serialize(process, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Processes data: {processJson}", "DEBUG");
                return await uow.OperationProcessRepository.BulkyUpdateAsync(process, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Processes : {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes: {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<OperationProcess, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes: {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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

        public async Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<OperationProcess, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
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
                    ErrorSource = "OPERATION-PROCESSES-SERVICE",
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
