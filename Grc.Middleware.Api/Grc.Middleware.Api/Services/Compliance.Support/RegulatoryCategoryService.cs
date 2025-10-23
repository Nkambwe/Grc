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
    public class RegulatoryCategoryService : BaseService, IRegulatoryCategoryService
    {
        public RegulatoryCategoryService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Categories in the database", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public int Count(Expression<Func<RegulatoryCategory, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Categories in the database", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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
            Logger.LogActivity($"Count number of Regulatory Categories in the database", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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
            Logger.LogActivity($"Count number of  Regulatory Categories in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<RegulatoryCategory, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Categories in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<int> CountAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Categories in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public bool Exists(Expression<Func<RegulatoryCategory, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Categories exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<bool> ExistsAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Categories exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryCategory, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Regulatory Categories if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Categories in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public RegulatoryCategory Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Category with ID '{id}'", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Category: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public RegulatoryCategory Get(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Category that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Category : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public RegulatoryCategory Get(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Categories that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public IQueryable<RegulatoryCategory> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Categories", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public IList<RegulatoryCategory> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Categories", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public IList<RegulatoryCategory> GetAll(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Categories that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryCategoryRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<IList<RegulatoryCategory>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Categories", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<IList<RegulatoryCategory>> GetAllAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Categories that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<IList<RegulatoryCategory>> GetAllAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Categories that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<IList<RegulatoryCategory>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Regulatory Categories", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<RegulatoryCategory> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Category with ID '{id}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Category : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<RegulatoryCategory> GetAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Category that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Category : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<RegulatoryCategory> GetAsync(Expression<Func<RegulatoryCategory, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Category that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Category : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<IList<RegulatoryCategory>> GetTopAsync(Expression<Func<RegulatoryCategory, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Regulatory Categories that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public bool Insert(RegulatoryCategoryRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Category request to Regulatory Category entity
                var category = Mapper.Map<RegulatoryCategoryRequest, RegulatoryCategory>(request);

                //..log the Regulatory Category data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Category data: {categoryJson}", "DEBUG");

                var added = uow.RegulatoryCategoryRepository.Insert(category);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save category : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<bool> InsertAsync(RegulatoryCategoryRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Category request to Regulatory Category entity
                var category = Mapper.Map<RegulatoryCategoryRequest, RegulatoryCategory>(request);

                //..log the Regulatory Category data being saved
                var categoryJson = JsonSerializer.Serialize(category, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Category data: {categoryJson}", "DEBUG");

                var added = await uow.RegulatoryCategoryRepository.InsertAsync(category);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Category : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public bool Update(RegulatoryCategoryRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Category request", "INFO");

            try
            {
                var category = uow.RegulatoryCategoryRepository.Get(a => a.Id == request.Id);
                if (category != null)
                {
                    //..update  Regulatory Category record
                    category.CategoryName = (request.CategoryName ?? string.Empty).Trim();
                    category.IsDeleted = request.IsDeleted;
                    category.LastModifiedOn = DateTime.Now;
                    category.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.RegulatoryCategoryRepository.Update(category, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($" Regulatory Category state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Regulatory Category record: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<bool> UpdateAsync(RegulatoryCategoryRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Category", "INFO");

            try
            {
                var category = await uow.RegulatoryCategoryRepository.GetAsync(a => a.Id == request.Id);
                if (category != null)
                {
                    //..update Regulatory Category record
                    category.CategoryName = (request.CategoryName ?? string.Empty).Trim();
                    category.IsDeleted = request.IsDeleted;
                    category.LastModifiedOn = DateTime.Now;
                    category.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.RegulatoryCategoryRepository.UpdateAsync(category, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(category).State;
                    Logger.LogActivity($"Regulatory Regulatory Category state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Regulatory Category record: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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
                var categoryJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Category data: {categoryJson}", "DEBUG");

                var categories = uow.RegulatoryCategoryRepository.Get(t => t.Id == request.RecordId);
                if (categories != null)
                {
                    //..mark as delete this  Regulatory Category
                    _ = uow.RegulatoryCategoryRepository.Delete(categories, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(categories).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Regulatory Category : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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
                var categoryJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Category data: {categoryJson}", "DEBUG");

                var typestask = await uow.RegulatoryCategoryRepository.GetAsync(t => t.Id == request.RecordId);
                if (typestask != null)
                {
                    //..mark as delete this  Regulatory Category
                    _ = await uow.RegulatoryCategoryRepository.DeleteAsync(typestask, request.IsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(typestask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Regulatory Category : {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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
                var categories = await uow.RegulatoryCategoryRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (categories.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.RegulatoryCategoryRepository.DeleteAllAsync(categories, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Regulatory Categories: {ex.Message}", "ERROR");

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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }

        }

        public async Task<bool> BulkyInsertAsync(RegulatoryCategoryRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map  Regulatory Categories to  Regulatory Categories entity
                var categories = requestItems.Select(Mapper.Map<RegulatoryCategoryRequest, RegulatoryCategory>).ToArray();
                var categoryJson = JsonSerializer.Serialize(categories, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Categories data: {categoryJson}", "DEBUG");
                return await uow.RegulatoryCategoryRepository.BulkyInsertAsync(categories);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Categories : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(RegulatoryCategoryRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Categories request to Regulatory Categories entity
                var categories = requestItems.Select(Mapper.Map<RegulatoryCategoryRequest, RegulatoryCategory>).ToArray();
                var categoryJson = JsonSerializer.Serialize(categories, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Categories data: {categoryJson}", "DEBUG");
                return await uow.RegulatoryCategoryRepository.BulkyUpdateAsync(categories);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Categories : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<bool> BulkyUpdateAsync(RegulatoryCategoryRequest[] requestItems, params Expression<Func<RegulatoryCategory, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Categories request to Regulatory Categories entity
                var categories = requestItems.Select(Mapper.Map<RegulatoryCategoryRequest, RegulatoryCategory>).ToArray();
                var categoryJson = JsonSerializer.Serialize(categories, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Categories data: {categoryJson}", "DEBUG");
                return await uow.RegulatoryCategoryRepository.BulkyUpdateAsync(categories, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Categories : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<PagedResult<RegulatoryCategory>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Categories", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.PageAllAsync(page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories: {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<PagedResult<RegulatoryCategory>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryCategory, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Categories", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<PagedResult<RegulatoryCategory>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryCategory, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Categories", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories: {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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

        public async Task<PagedResult<RegulatoryCategory>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryCategory, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Categories", "INFO");

            try
            {
                return await uow.RegulatoryCategoryRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Categories : {ex.Message}", "ERROR");
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
                    ErrorSource = "REGULATORY-CATEGORY-SERVICE",
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
