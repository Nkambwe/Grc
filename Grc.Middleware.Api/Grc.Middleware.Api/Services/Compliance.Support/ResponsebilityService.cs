using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Support {

    public class ResponsebilityService : BaseService, IResponsebilityService
    {
        public ResponsebilityService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Responsebilities in the database", "INFO");

            try
            {
                return uow.ResponsebilityRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public int Count(Expression<Func<Responsebility, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Responsebilities in the database", "INFO");

            try
            {
                return uow.ResponsebilityRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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
            Logger.LogActivity($"Count number of Responsebilities in the database", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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
            Logger.LogActivity($"Count number of  Responsebilities in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<Responsebility, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Responsebilities in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<Responsebility, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Responsebilities in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public bool Exists(Expression<Func<Responsebility, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Responsebilities exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ResponsebilityRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<bool> ExistsAsync(Expression<Func<Responsebility, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Responsebilities exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<Responsebility, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Responsebilities if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Responsebilities in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public Responsebility Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get  Responsebility with ID '{id}'", "INFO");

            try
            {
                return uow.ResponsebilityRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve  Responsebility: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public Responsebility Get(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get  Responsebility that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ResponsebilityRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve  Responsebility : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public Responsebility Get(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Responsebilities that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ResponsebilityRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public IQueryable<Responsebility> GetAll(bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Responsebilities", "INFO");

            try
            {
                return uow.ResponsebilityRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public IList<Responsebility> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Responsebilities", "INFO");

            try
            {
                return uow.ResponsebilityRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public IList<Responsebility> GetAll(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Responsebilities that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.ResponsebilityRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<IList<Responsebility>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Responsebilities", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<IList<Responsebility>> GetAllAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Responsebilities that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<IList<Responsebility>> GetAllAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Responsebilities that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<IList<Responsebility>> GetAllAsync(bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Responsebilities", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<Responsebility> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Responsebility with ID '{id}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebility : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<Responsebility> GetAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Responsebility that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebility : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<Responsebility> GetAsync(Expression<Func<Responsebility, bool>> predicate, bool includeDeleted = false, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Responsebility that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebility : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<IList<Responsebility>> GetTopAsync(Expression<Func<Responsebility, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Responsebilities that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public bool Insert(ResponsebilityRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Responsebility request to  Responsebility entity
                var responsebility = Mapper.Map<ResponsebilityRequest, Responsebility>(request);

                //..log the Responsebility data being saved
                var reseponsebilityJson = JsonSerializer.Serialize(responsebility, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebility data: {reseponsebilityJson}", "DEBUG");

                var added = uow.ResponsebilityRepository.Insert(responsebility);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(responsebility).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save submission : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<bool> InsertAsync(ResponsebilityRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map  Responsebility request to  Responsebility entity
                var resposebility = Mapper.Map<ResponsebilityRequest, Responsebility>(request);

                //..log the Responsebility data being saved
                var typeJson = JsonSerializer.Serialize(resposebility, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebility data: {typeJson}", "DEBUG");

                var added = await uow.ResponsebilityRepository.InsertAsync(resposebility);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(resposebility).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Responsebility : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public bool Update(ResponsebilityRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Responsebility request", "INFO");

            try
            {
                var responsebility = uow.ResponsebilityRepository.Get(a => a.Id == request.Id);
                if (responsebility != null)
                {
                    //..update  Responsebility record
                    responsebility.ContactName = (request.ContactName ?? string.Empty).Trim();
                    responsebility.ContactPhone = (request.ContactPhone ?? string.Empty).Trim();
                    responsebility.ContactEmail = (request.ContactEmail ?? string.Empty).Trim();
                    responsebility.ContactPosition = (request.ContactPosition ?? string.Empty).Trim();
                    responsebility.Description = (request.Description ?? string.Empty).Trim();
                    responsebility.DepartmentId = request.DepartmentId;
                    responsebility.IsDeleted = request.IsDeleted;
                    responsebility.LastModifiedOn = DateTime.Now;
                    responsebility.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.ResponsebilityRepository.Update(responsebility, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(responsebility).State;
                    Logger.LogActivity($"Responsebility state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update  Responsebility record: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<bool> UpdateAsync(ResponsebilityRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Responsebility", "INFO");

            try
            {
                var responsebility = await uow.ResponsebilityRepository.GetAsync(a => a.Id == request.Id);
                if (responsebility != null)
                {
                    //..update  Responsebility record
                    responsebility.ContactName = (request.ContactName ?? string.Empty).Trim();
                    responsebility.ContactPhone = (request.ContactPhone ?? string.Empty).Trim();
                    responsebility.ContactEmail = (request.ContactEmail ?? string.Empty).Trim();
                    responsebility.ContactPosition = (request.ContactPosition ?? string.Empty).Trim();
                    responsebility.Description = (request.Description ?? string.Empty).Trim();
                    responsebility.DepartmentId = request.DepartmentId;
                    responsebility.IsDeleted = request.IsDeleted;
                    responsebility.LastModifiedOn = DateTime.Now;
                    responsebility.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.ResponsebilityRepository.UpdateAsync(responsebility, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(responsebility).State;
                    Logger.LogActivity($"Regulatory Responsebility state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Responsebility record: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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
                var responsebilityJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebility data: {responsebilityJson}", "DEBUG");

                var responsebility = uow.ResponsebilityRepository.Get(t => t.Id == request.RecordId);
                if (responsebility != null)
                {
                    //..mark as delete this Responsebility
                    _ = uow.ResponsebilityRepository.Delete(responsebility, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(responsebility).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete  Responsebility : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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
                var responsebilityJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebility data: {responsebilityJson}", "DEBUG");

                var responsebilitytask = await uow.ResponsebilityRepository.GetAsync(t => t.Id == request.RecordId);
                if (responsebilitytask != null)
                {
                    //..mark as delete this Responsebility
                    _ = await uow.ResponsebilityRepository.DeleteAsync(responsebilitytask, request.IsDeleted);

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
                Logger.LogActivity($"Failed to delete  Responsebility : {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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
                var types = await uow.ResponsebilityRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (types.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.ResponsebilityRepository.DeleteAllAsync(types, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Responsebilities: {ex.Message}", "ERROR");

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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }

        }

        public async Task<bool> BulkyInsertAsync(ResponsebilityRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Responsebilities to Responsebilities entity
                var responsebilities = requestItems.Select(Mapper.Map<ResponsebilityRequest, Responsebility>).ToArray();
                var responsebilityJson = JsonSerializer.Serialize(responsebilities, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebilities data: {responsebilityJson}", "DEBUG");
                return await uow.ResponsebilityRepository.BulkyInsertAsync(responsebilities);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Responsebilities : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(ResponsebilityRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Responsebilities request to  Responsebilities entity
                var responsebilities = requestItems.Select(Mapper.Map<ResponsebilityRequest, Responsebility>).ToArray();
                var responsebilityJson = JsonSerializer.Serialize(responsebilities, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebilities data: {responsebilityJson}", "DEBUG");
                return await uow.ResponsebilityRepository.BulkyUpdateAsync(responsebilities);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Responsebilities : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(ResponsebilityRequest[] requestItems, params Expression<Func<Responsebility, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map  Responsebilities request to  Responsebilities entity
                var responsebility = requestItems.Select(Mapper.Map<ResponsebilityRequest, Responsebility>).ToArray();
                var responseJson = JsonSerializer.Serialize(responsebility, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Responsebilities data: {responseJson}", "DEBUG");
                return await uow.ResponsebilityRepository.BulkyUpdateAsync(responsebility, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Responsebilities : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<PagedResult<Responsebility>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Responsebilities", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.PageAllAsync(page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities: {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<PagedResult<Responsebility>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<Responsebility, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Responsebilities", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<PagedResult<Responsebility>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<Responsebility, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Responsebilities", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities: {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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

        public async Task<PagedResult<Responsebility>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<Responsebility, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Responsebilities", "INFO");

            try
            {
                return await uow.ResponsebilityRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Responsebilities : {ex.Message}", "ERROR");
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
                    ErrorSource = "RESPONSEBILITY-SERVICE",
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
