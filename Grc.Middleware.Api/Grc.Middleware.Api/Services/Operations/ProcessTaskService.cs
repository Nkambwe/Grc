using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Audits;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Operations {
    public class ProcessTaskService : BaseService, IProcessTaskService
    {
        public ProcessTaskService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }


        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of process tasks in the database", "INFO");

            try
            {
                return uow.ProcessTaskRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public int Count(Expression<Func<ProcessTask, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of process taskss in the database", "INFO");

            try
            {
                return uow.ProcessTaskRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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
            Logger.LogActivity($"Count number of process tasks in the database", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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
            Logger.LogActivity($"Count number of process tasks in the database", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<ProcessTask, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of process tasks in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<ProcessTask, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of process tasks in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public bool Exists(Expression<Func<ProcessTask, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an process task exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ProcessTaskRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for process task in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<bool> ExistsAsync(Expression<Func<ProcessTask, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an process tasks exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for process task in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessTask, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch process tasks if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for process tasks in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public ProcessTask Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get process task with ID '{id}'", "INFO");

            try
            {
                return uow.ProcessTaskRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public ProcessTask Get(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get process task that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ProcessTaskRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public ProcessTask Get(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get process task that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ProcessTaskRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public IQueryable<ProcessTask> GetAll(bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all process tasks", "INFO");

            try
            {
                return uow.ProcessTaskRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public IList<ProcessTask> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all process tasks", "INFO");

            try
            {
                return uow.ProcessTaskRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public IList<ProcessTask> GetAll(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all process task that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.ProcessTaskRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<IList<ProcessTask>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all process tasks", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<IList<ProcessTask>> GetAllAsync(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<IList<ProcessTask>> GetAllAsync(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all process tasks that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve processes task: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<IList<ProcessTask>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all process tasks", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<ProcessTask> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get process task with ID '{id}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<ProcessTask> GetAsync(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get process task that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<ProcessTask> GetAsync(Expression<Func<ProcessTask, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get process task that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<IList<ProcessTask>> GetTopAsync(Expression<Func<ProcessTask, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} process tasks that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public bool Insert(ProcessTaskRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map process task request to process task entity
                var task = Mapper.Map<ProcessTaskRequest, ProcessTask>(request);

                //..log the process task data being saved
                var taskJson = JsonSerializer.Serialize(task, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");

                var added = uow.ProcessTaskRepository.Insert(task);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save process task : {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<bool> InsertAsync(ProcessTaskRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map process task request to process task entity
                var task = Mapper.Map<ProcessTaskRequest, ProcessTask>(request);

                //..log the process task data being saved
                var taskJson = JsonSerializer.Serialize(task, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");

                var added = await uow.ProcessTaskRepository.InsertAsync(task);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save process task : {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public bool Update(ProcessTaskRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update process task ", "INFO");

            try
            {
                var task = uow.ProcessTaskRepository.Get(a => a.Id == request.Id);
                if (task != null)
                {
                    //..update process task record
                    task.TaskName = (request.TaskName ?? string.Empty).Trim();
                    task.Description = (request.Description ?? string.Empty).Trim();
                    task.Status = (request.Status ?? string.Empty).Trim();
                    task.DueDate = request.DueDate;
                    task.ProcessId = request.ProcessId;
                    task.IsDeleted = request.IsDeleted;
                    task.LastModifiedOn = DateTime.Now;
                    task.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.ProcessTaskRepository.Update(task, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Process task state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update process task record: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<bool> UpdateAsync(ProcessTaskRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update process task", "INFO");

            try
            {
                var task = await uow.ProcessTaskRepository.GetAsync(a => a.Id == request.Id);
                if (task != null)
                {
                    //..update process record
                    task.TaskName = (request.TaskName ?? string.Empty).Trim();
                    task.Description = (request.Description ?? string.Empty).Trim();
                    task.Status = (request.Status ?? string.Empty).Trim();
                    task.DueDate = request.DueDate;
                    task.ProcessId = request.ProcessId;
                    task.IsDeleted = request.IsDeleted;
                    task.LastModifiedOn = DateTime.Now;
                    task.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.ProcessTaskRepository.UpdateAsync(task, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Process task state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update process task record: {ex.Message}", "ERROR");

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
                    ErrorSource = "AUDIT-TASk-SERVICE",
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
                var taskJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");

                var task = uow.ProcessTaskRepository.Get(t => t.Id == request.RecordId);
                if (task != null)
                {
                    //..mark as delete this process task
                    _ = uow.ProcessTaskRepository.Delete(task, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(task).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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
                var taskJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");

                var tasktask = await uow.ProcessTaskRepository.GetAsync(t => t.Id == request.RecordId);
                if (tasktask != null)
                {
                    //..mark as delete this process task
                    _ = await uow.ProcessTaskRepository.DeleteAsync(tasktask, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(tasktask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process task : {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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
                var processes = await uow.ProcessTaskRepository.GetAllAsync(e => requesItems.Contains(e.Id));
                if (processes.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.ProcessTaskRepository.DeleteAllAsync(processes, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process: {ex.Message}", "ERROR");

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
                    ErrorSource = "PROCESS-TASK-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(ProcessTaskRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map process task request to process task entity
                var tasks = requestItems.Select(Mapper.Map<ProcessTaskRequest, ProcessTask>).ToArray();
                var taskJson = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");
                return await uow.ProcessTaskRepository.BulkyInsertAsync(tasks);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save process : {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(ProcessTaskRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map process task request to process task entity
                var task = requestItems.Select(Mapper.Map<ProcessTaskRequest, ProcessTask>).ToArray();
                var taskJson = JsonSerializer.Serialize(task, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");
                return await uow.ProcessTaskRepository.BulkyUpdateAsync(task);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save process task : {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(ProcessTaskRequest[] requestItems, params Expression<Func<ProcessTask, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map process task request to process task entity
                var task = requestItems.Select(Mapper.Map<ProcessTaskRequest, ProcessTask>).ToArray();
                var taskJson = JsonSerializer.Serialize(task, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process task data: {taskJson}", "DEBUG");
                return await uow.ProcessTaskRepository.BulkyUpdateAsync(task, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save process tasks : {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<PagedResult<ProcessTask>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged process tasks", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.PageAllAsync(page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks: {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<PagedResult<ProcessTask>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessTask, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged process tasks ", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks: {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<PagedResult<ProcessTask>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessTask, bool>> where = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged process task", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.PageAllAsync(page, size, includeDeleted, where);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks: {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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

        public async Task<PagedResult<ProcessTask>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessTask, bool>> where = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged process tasks", "INFO");

            try
            {
                return await uow.ProcessTaskRepository.PageAllAsync(token, page, size, where, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve process tasks : {ex.Message}", "ERROR");
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
                    ErrorSource = "PROCESS-TASK-SERVICE",
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
