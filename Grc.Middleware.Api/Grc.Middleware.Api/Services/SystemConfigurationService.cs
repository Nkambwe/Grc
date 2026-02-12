using AutoMapper;
using Azure;
using Azure.Core;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using Newtonsoft.Json.Linq;
using RTools_NTS.Util;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Grc.Middleware.Api.Services
{
    public class SystemConfigurationService : BaseService, ISystemConfigurationService
    {
        public SystemConfigurationService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<bool> ExistsAsync(Expression<Func<SystemConfiguration, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if configuration that fits predicate >> '{predicate}' exists", "INFO");

            try {
                return await uow.SystemConfigurationRepository.ExistsAsync(predicate, excludeDeleted, token);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system configuration: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<PasswordChangeResponse> GetPasswordSettingAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieving password settings", "INFO");

            try {
                var settings = await GetPasswordSettingsFromDatabase(uow);
                return MapToPasswordChangeResponse(settings);
            } catch (Exception ex) {
                LogExceptionWithInnerExceptions(ex, "Failed to retrieve password settings");
                throw;
            }
        }

        public async Task<ConfigurationResponse> GetAllConfigurationAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all application configurations", "INFO");

            var response = new ConfigurationResponse() {
                GeneralSettings = new(),
                PolicySettings = new(),
                AuditSettings = new(),
                ObligationSettings = new(),
                MappingSettings = new(),
                SecuritySettings = new()
            };
            try {
                var settings = (await uow.SystemConfigurationRepository.GetAllAsync(true))
                               ?.ToDictionary(s => s.ParameterName, s => s.ParameterValue)
                               ?? new Dictionary<string, string>();

                //..general
                response.GeneralSettings.SoftDeleteRecord = ToBool(settings.GetValueOrDefault("GENERAL_SOFTDELETERECORD"));
                response.GeneralSettings.IncludeDeletedRecords = ToBool(settings.GetValueOrDefault("GENERAL_INCLUDEDELETEDRECORD"));

                //..policy seettings
                response.PolicySettings.SendNotifications = ToBool(settings.GetValueOrDefault("POLICY_SENDNOTIFICATIONS"));
                response.PolicySettings.MaximumNotifications = ToInt(settings.GetValueOrDefault("POLICY_MAXIMUMNUMBEROFNOTIFICATIONS"));

                //..audit
                response.AuditSettings.SendNotifications = ToBool(settings.GetValueOrDefault("AUDIT_SENDNOTIFICATIONS"));
                response.AuditSettings.MaximumNotifications = ToInt(settings.GetValueOrDefault("AUDIT_MAXIMUMNUMBEROFNOTIFICATIONS"));

                //..obligations
                response.ObligationSettings.SendNotifications = ToBool(settings.GetValueOrDefault("OBLIGATION_SENDNOTIFICATIONS"));
                response.ObligationSettings.MaximumNotifications = ToInt(settings.GetValueOrDefault("OBLIGATION_MAXIMUMNUMBEROFNOTIFICATIONS"));

                //..mapping
                response.MappingSettings.UsersCanAddComplianceControls = ToBool(settings.GetValueOrDefault("MAPPING_USERSCANADDCOMPLIANCECONTROLS"));
                response.MappingSettings.LimitNumberOfItemsOnControl = ToBool(settings.GetValueOrDefault("MAPPING_LIMITNUMBEROFCONTROLS"));
                response.MappingSettings.MaximumNumberOfItemsOnControl = ToInt(settings.GetValueOrDefault("MAPPING_MAXIMUMNUMBEROFCONTROLITEMS"));

                //..security
                response.SecuritySettings.ExpirePassword = ToBool(settings.GetValueOrDefault("SECURITY_EXPIRPASSWORDS"));
                response.SecuritySettings.ExipreyPeriod = ToInt(settings.GetValueOrDefault("SECURITY_EXPIRYPERIOD"));
                response.SecuritySettings.CanUseOldPassword = ToBool(settings.GetValueOrDefault("SECURITY_CANUSEOLDPASSWORDS"));
                response.SecuritySettings.AllowAdmininsToResetPasswords = ToBool(settings.GetValueOrDefault("SECURITY_ALLOWMANUALPASSWORDCHANGE"));
                response.SecuritySettings.AllowPasswordReuse = ToBool(settings.GetValueOrDefault("SECURITY_ALLOWMANUALPASSWORDCHANGE"));
                response.SecuritySettings.MinimumPasswordLength = ToInt(settings.GetValueOrDefault("SECURITY_MINIMUMPASSWORDLENGTH"));
                response.SecuritySettings.IncludeUpperCharacters = ToBool(settings.GetValueOrDefault("SECURITY_INCLUDEUPPERCASECHAR"));
                response.SecuritySettings.IncludeLowerCharacters = ToBool(settings.GetValueOrDefault("SECURITY_INCLUDELOWERCASECHAR"));
                response.SecuritySettings.IncludeSpecialCharacters = ToBool(settings.GetValueOrDefault("SECURITY_INCLUDELOWERSPECIALCHAR"));
                response.SecuritySettings.IncludeNumericCharacters = ToBool(settings.GetValueOrDefault("SECURITY_INCLUDEONENUMBER"));

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system configuration: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<ConfigurationParameterResponse<T>> GetConfigurationAsync<T>(string paramName) {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve System Configuration", "INFO");

            try {
                var config = await uow.SystemConfigurationRepository.GetAsync(p => p.ParameterName == paramName);
                if (config == null)
                    return null;

                var value = ConvertValue(config);

                return new ConfigurationParameterResponse<T> {
                    ParameterName = config.ParameterName,
                    ParameterType = config.ParameterType,
                    Value = (T)value
                };
            } catch (Exception ex) {
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

        public async Task<bool> UpdateConfigurationAsync(SystemConfigurationRequest request, string username) {
            using var uow = UowFactory.Create();

            var config = await uow.SystemConfigurationRepository.GetAsync(s=>s.ParameterName == request.ParamName);
            var normalizedValue = NormalizeValue(request.ParamValue, config.ParameterType);

            //..only update if changed
            if (config.ParameterValue != normalizedValue) {
                config.ParameterValue = normalizedValue;
                config.LastModifiedBy = username;
                config.LastModifiedOn = DateTime.UtcNow;

                uow.SystemConfigurationRepository.Update(config);
                await uow.SaveChangesAsync();

                Logger.LogActivity($"Updated configuration {request.ParamName} to {normalizedValue}","INFO");
                return true;
            }

            return false;
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

            try {
                return await uow.SystemConfigurationRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            } catch (Exception ex) {
                _ = await uow.SystemErrorRespository.InsertAsync(CreateErrorObject(uow, ex));
                throw;
            }
        }

        public async Task<bool> SavePolicyConfigurationsAsync(PolicyConfigurationsRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save policy configurations", "INFO");

            try {
                var settings = await uow.SystemConfigurationRepository.GetAllAsync(s => s.ParameterName.StartsWith("POLICY"), true);
                if (settings == null || !settings.Any()) {
                    return false;
                }

                //..convert to dictionary for lookups
                var settingsDict = settings.ToDictionary(s => s.ParameterName);
                var now = DateTime.Now;
                var entitiesToUpdate = new List<SystemConfiguration>(settingsDict.Count);

                //..define configuration updates
                var updates = new[] {
                    ("POLICY_SENDNOTIFICATIONS", request.SendPolicyNotifications.ToString(), "FLAG"),
                    ("POLICY_MAXIMUMNUMBEROFNOTIFICATIONS", request.MaximumNumberOfNotifications.ToString(), "NUMBER")
                };

                foreach (var (paramName, newValue, paramType) in updates) {
                    if (settingsDict.TryGetValue(paramName, out var setting)) {
                        var normalizedValue = NormalizeValue(newValue, paramType);
                        if (setting.ParameterValue != normalizedValue) {
                            setting.ParameterValue = normalizedValue;
                            setting.LastModifiedBy = username;
                            setting.LastModifiedOn = now;
                            entitiesToUpdate.Add(setting);
                        }
                    }
                }

                if (entitiesToUpdate.Count > 0) {
                    return await uow.SystemConfigurationRepository.BulkyUpdateAsync(entitiesToUpdate.ToArray());
                }

                return false;
            } catch (Exception ex) {
                _ = await uow.SystemErrorRespository.InsertAsync(CreateErrorObject(uow, ex));
                throw;
            }
        }

        public async Task<bool> SavePasswordPolicyConfigurationsAsync(PasswordConfigurationsRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save policy configurations", "INFO");

            try {
                var settings = await uow.SystemConfigurationRepository.GetAllAsync(s => s.ParameterName.StartsWith("SECURITY"), true);
                if (settings == null || !settings.Any()) {
                    return false;
                }

                //..convert to dictionary for lookups
                var settingsDict = settings.ToDictionary(s => s.ParameterName);
                var now = DateTime.Now;
                var entitiesToUpdate = new List<SystemConfiguration>(settingsDict.Count);

                //..define configuration updates
                var updates = new[] {
                    ("SECURITY_EXPIRPASSWORDS", request.EnforcePasswordExpiration.ToString(), "FLAG"),
                    ("SECURITY_EXPIRYPERIOD", request.DaysUntilPasswordExpiration.ToString(), "NUMBER"),
                    ("SECURITY_CANUSEOLDPASSWORDS", request.AllowPasswordReuse.ToString(), "FLAG"),
                    ("SECURITY_ALLOWMANUALPASSWORDCHANGE", request.AllowManualPasswordReset.ToString(), "FLAG"),
                    ("SECURITY_MINIMUMPASSWORDLENGTH", request.MinimumPasswordLength.ToString(), "NUMBER"),
                    ("SECURITY_INCLUDEUPPERCASECHAR", request.IncludeUppercaseCharacters.ToString(), "FLAG"),
                    ("SECURITY_INCLUDELOWERCASECHAR", request.IncludeLowercaseCharacters.ToString(), "FLAG"),
                    ("SECURITY_INCLUDELOWERSPECIALCHAR", request.IncludeSpecialCharacters.ToString(), "FLAG"),
                    ("SECURITY_INCLUDEONENUMBER", request.IncludeNumericCharacters.ToString(), "FLAG"),
                };

                foreach (var (paramName, newValue, paramType) in updates) {
                    if (settingsDict.TryGetValue(paramName, out var setting)) {
                        var normalizedValue = NormalizeValue(newValue, paramType);
                        if (setting.ParameterValue != normalizedValue) {
                            setting.ParameterValue = normalizedValue;
                            setting.LastModifiedBy = username;
                            setting.LastModifiedOn = now;
                            entitiesToUpdate.Add(setting);
                        }
                    }
                }

                if (entitiesToUpdate.Count > 0) {
                    return await uow.SystemConfigurationRepository.BulkyUpdateAsync(entitiesToUpdate.ToArray());
                }

                return false;
            } catch (Exception ex) {
                _ = await uow.SystemErrorRespository.InsertAsync(CreateErrorObject(uow, ex));
                throw;
            }
        }

        public async Task<bool> SaveGeneralConfigurationsAsync(GeneralConfigurationsRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save general configurations", "INFO");

            try {
                var settings = await uow.SystemConfigurationRepository.GetAllAsync(s => s.ParameterName.StartsWith("GENERAL"), true);
                if (settings == null || !settings.Any()) {
                    return false;
                }

                //..convert to dictionary for lookups
                var settingsDict = settings.ToDictionary(s => s.ParameterName);
                var now = DateTime.Now;
                var entitiesToUpdate = new List<SystemConfiguration>(settingsDict.Count);

                //..define configuration updates
                var updates = new[] {
                    ("GENERAL_SOFTDELETERECORD", request.SoftDeleteRecords.ToString(), "FLAG"),
                    ("GENERAL_INCLUDEDELETEDRECORD", request.IncludeDeletedRecord.ToString(), "FLAG")
                };

                foreach (var (paramName, newValue, paramType) in updates) {
                    if (settingsDict.TryGetValue(paramName, out var setting)) {
                        var normalizedValue = NormalizeValue(newValue, paramType);
                        if (setting.ParameterValue != normalizedValue) {
                            setting.ParameterValue = normalizedValue;
                            setting.LastModifiedBy = username;
                            setting.LastModifiedOn = now;
                            entitiesToUpdate.Add(setting);
                        }
                    }
                }

                if (entitiesToUpdate.Count > 0) {
                    return await uow.SystemConfigurationRepository.BulkyUpdateAsync(entitiesToUpdate.ToArray());
                }

                return false;
            } catch (Exception ex) {
                _ = await uow.SystemErrorRespository.InsertAsync(CreateErrorObject(uow, ex));
                throw;
            }
        }

        #region Private Methods
        private async Task<Dictionary<string, string>> GetPasswordSettingsFromDatabase(IUnitOfWork uow) {
            var passwordParameters = new[] {
                "SECURITY_CANUSEOLDPASSWORDS",
                "SECURITY_MINIMUMPASSWORDLENGTH",
                "SECURITY_INCLUDEUPPERCASECHAR",
                "SECURITY_INCLUDELOWERCASECHAR",
                "SECURITY_INCLUDELOWERSPECIALCHAR",
                "SECURITY_INCLUDEONENUMBER"
            };

            var settingsList = await uow.SystemConfigurationRepository
                .GetAllAsync(s => passwordParameters.Contains(s.ParameterName), true);

            return settingsList?.ToDictionary(s => s.ParameterName, s => s.ParameterValue)
                   ?? new Dictionary<string, string>();
        }

        private PasswordChangeResponse MapToPasswordChangeResponse(Dictionary<string, string> settings) {
            return new PasswordChangeResponse {
                MinimumLength = GetIntSetting(settings, "SECURITY_MINIMUMPASSWORDLENGTH", defaultValue: 12),
                IncludeUpperChar = GetBoolSetting(settings, "SECURITY_CANUSEOLDPASSWORDS", defaultValue: true),
                IncludeLowerChar = GetBoolSetting(settings, "SECURITY_INCLUDEUPPERCASECHAR", defaultValue: true),
                IncludeSpecialChar = GetBoolSetting(settings, "SECURITY_INCLUDELOWERCASECHAR", defaultValue: true),
                CanReusePasswords = GetBoolSetting(settings, "SECURITY_INCLUDELOWERSPECIALCHAR", defaultValue: false),
                IncludeNumericChar = GetBoolSetting(settings, "SECURITY_INCLUDEONENUMBER", defaultValue: false)
            };
        }

        private bool GetBoolSetting(Dictionary<string, string> settings, string key, bool defaultValue) {
            var value = settings.GetValueOrDefault(key);
            return string.IsNullOrEmpty(value) ? defaultValue : ToBool(value);
        }

        private int GetIntSetting(Dictionary<string, string> settings, string key, int defaultValue) {
            var value = settings.GetValueOrDefault(key);
            return string.IsNullOrEmpty(value) ? defaultValue : ToInt(value);
        }

        private void LogExceptionWithInnerExceptions(Exception ex, string message) {
            Logger.LogActivity($"{message}: {ex.Message}", "ERROR");

            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
        }
        private static string NormalizeValue(string value, string parameterType) {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return parameterType switch {
                "FLAG" => NormalizeBool(value),
                "NUMBER" => NormalizeInt(value),
                "MONEY" => NormalizeDecimal(value),
                "TEXT" => value.Trim(),
                "DATETIME" => NormalizeDateTime(value),
                "TIME" => value.Trim(),
                _ => throw new InvalidOperationException($"Unsupported parameter type: {parameterType}")
            };
        }

        private static string NormalizeBool(string value) {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value == "1"
                ? "YES"
                : "NO";
        }

        private static string NormalizeInt(string value) {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                throw new ArgumentException("Invalid number value");

            return i.ToString(CultureInfo.InvariantCulture);
        }
        private static string NormalizeDecimal(string value) {
            var cleaned = value
                .Replace(",", "")
                .Trim();

            if (!decimal.TryParse(cleaned, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var d)) {
                throw new ArgumentException("Invalid money value");
            }

            return d.ToString("0.00", CultureInfo.InvariantCulture);
        }
        private static string NormalizeDateTime(string value) {
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                throw new ArgumentException("Invalid datetime value");

            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static bool ToBool(string value)
            => value?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true || value == "1";

        private static int ToInt(string value)
            => int.TryParse(value, out var i) ? i : 0;

        private static decimal ToDecimal(string value)
            => decimal.TryParse(value, out var d) ? d : 0m;

        private static object ConvertValue(SystemConfiguration config) {
            return config.ParameterType switch {
                "FLAG" => config.ParameterValue.Equals("YES", StringComparison.OrdinalIgnoreCase),
                "NUMBER" => int.Parse(config.ParameterValue),
                "MONEY" => decimal.Parse(config.ParameterValue, CultureInfo.InvariantCulture),
                "TEXT" => config.ParameterValue,
                "DATETIME" => DateTime.Parse(config.ParameterValue, CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException($"Unsupported type {config.ParameterType}")
            };
        }

        private SystemError CreateErrorObject(IUnitOfWork uow, Exception ex) {
            Logger.LogActivity($"Failed to retrieve system configurations : {ex.Message}", "ERROR");
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
            Logger.LogActivity($"{ex.StackTrace}", "ERROR");

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            return new SystemError() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "SYSTEM-CONFIGURATION-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

        }
        #endregion

    }
}
