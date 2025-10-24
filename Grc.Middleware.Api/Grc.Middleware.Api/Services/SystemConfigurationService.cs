using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services
{
    public class SystemConfigurationService : BaseService, ISystemConfigurationService
    {
        public SystemConfigurationService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<SystemConfiguration> GetConfigurationAsync(string paramName)
        {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve System Configuration", "INFO");

            try
            {
                Logger.LogActivity($"System configuration ParaName: {paramName}", "DEBUG");
                var user = await uow.SystemConfigurationRepository.GetAsync(p => p.ParameterName == paramName);

                //..log system configuration
                var paramJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"System configuration record: {paramJson}", "DEBUG");

                return user;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to System configuration: {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-CONFIGURATION-SERVICE",
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

        public async Task<bool> UpdateConfigurationAsync(SystemConfigurationRequest request)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Update System Configuration request", "INFO");

            try
            {
                var config = await uow.SystemConfigurationRepository.GetAsync(a => a.ParameterName == request.ParamName);
                if (config != null)
                {
                    //..update System configurations
                    config.ParameterValue = (request.ParamValue ?? string.Empty).Trim();
                    config.ParameterType = (request.ParamType ?? string.Empty).Trim();
                    config.LastModifiedOn = DateTime.Now;
                    config.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.SystemConfigurationRepository.UpdateAsync(config);
                    var entityState = ((UnitOfWork)uow).Context.Entry(config).State;
                    Logger.LogActivity($"System Configuration state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update System Configuration password: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-CONFIGURATION-SERVICE",
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

        public async Task<IList<SystemConfiguration>> GetAllAsync(Expression<Func<SystemConfiguration, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all System Configurations", "INFO");

            try
            {
                return await uow.SystemConfigurationRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system configurations : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-CONFIGURATION-SERVICE",
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

        public async Task<PagedResult<SystemConfiguration>> PagedUsersAsync(CancellationToken token, int page, int size, Expression<Func<SystemConfiguration, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all system configurations", "INFO");

            try
            {
                return await uow.SystemConfigurationRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system configurations : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-CONFIGURATION-SERVICE",
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
