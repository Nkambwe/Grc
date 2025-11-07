using AutoMapper;
using Azure.Core;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services {

    public class SystemAccessService : BaseService, ISystemAccessService {

        public SystemAccessService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
            : base(loggerFactory, unitOfWorkFactory, mapper) {
        }

        #region Admin Dashboard
        public async Task<int> GetTotalUsersCountAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Total User Count", "INFO");

            try {
                return await uow.UserRepository.CountAsync(false);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve total user count: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<int> GetActiveUsersCountAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Active User Count", "INFO");

            try {
                return await uow.UserRepository.CountAsync(u => u.IsActive, false);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve active user count: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<AdminCountResponse> GetAdminiDashboardStatisticsAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Admin Dashboard Statistics", "INFO");

            try {
                // Get all user counts sequentially
                var totalUsers = await uow.UserRepository.CountAsync(false);
                var activeUsers = await uow.UserRepository.CountAsync(u => u.IsActive, true);
                var deactivatedUsers = await uow.UserRepository.CountAsync(u => !u.IsActive, true);
                var unApprovedUsers = await uow.UserRepository.CountAsync(u => !(bool)u.IsApproved, true);
                var unverifiedUsers = await uow.UserRepository.CountAsync(u => !(bool)u.IsVerified, true);
                var deletedUsers = await uow.UserRepository.CountAsync(u => u.IsDeleted, true);

                // Get all bug counts sequentially
                var totalBugs = await uow.SystemErrorRespository.CountAsync(false);
                var newBugs = await uow.SystemErrorRespository.CountAsync(b => b.FixStatus == "OPEN", false);
                var bugFixes = await uow.SystemErrorRespository.CountAsync(b => b.FixStatus == "CLOSED", false);
                var bugProgressTask = await uow.SystemErrorRespository.CountAsync(b => b.FixStatus == "PROGRESS", false);
                var userReportedBugsTask = await uow.SystemErrorRespository.CountAsync(b => b.IsUserReported, false);

                return new AdminCountResponse {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    DeactivatedUsers = deactivatedUsers,
                    UnApprovedUsers = unApprovedUsers,
                    UnverifiedUsers = unverifiedUsers,
                    DeletedUsers = deletedUsers,
                    TotalBugs = totalBugs,
                    NewBugs = newBugs,
                    BugFixes = bugFixes,
                    BugProgress = bugProgressTask,
                    UserReportedBugs = userReportedBugsTask
                };
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve admin dashboard statistics: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        #endregion

        #region System Access
        public async Task<UsernameValidationResponse> ValidateUsernameAsync(string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Validating username: {username}", "INFO");

            try {
                var user = await uow.UserRepository.GetAsync(u => u.Username.ToLower() == username.ToLower());

                if (user == null) {
                    Logger.LogActivity($"Username not found: {username}", "DEBUG");
                    return new UsernameValidationResponse {
                        IsValid = false,
                        Message = "Username not found",
                        DisplayName = string.Empty,
                        UserId = 0
                    };
                }

                // Check if user is active
                if (!user.IsActive) {
                    Logger.LogActivity($"Inactive user attempted login: {username}", "WARN");
                    return new UsernameValidationResponse {
                        IsValid = false,
                        Message = "User account is inactive",
                        DisplayName = string.Empty,
                        UserId = 0
                    };
                }

                Logger.LogActivity($"Username validation successful: {username}", "DEBUG");
                return new UsernameValidationResponse {
                    IsValid = true,
                    Message = "Username found",
                    DisplayName = user.FirstName.Trim(),
                    UserId = user.Id
                };
            } catch (Exception ex) {
                Logger.LogActivity($"Username validation failed: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                return new UsernameValidationResponse {
                    IsValid = false,
                    Message = "Service unavailable. Please try again later.",
                    DisplayName = string.Empty,
                    UserId = 0
                };
            }
        }

        public async Task<AuthenticationResponse> AuthenticateUserAsync(string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Validating username: {username}", "INFO");

            try {
                //..get user record
                var user = await uow.UserRepository.GetAsync(u => u.Username.ToLower() == username.ToLower(), true, u => u.Role, u => u.Department);
                if (user == null) {
                    Logger.LogActivity($"Username not found: {username}", "DEBUG");
                    return new AuthenticationResponse() {
                        RedirectUrl = string.Empty,
                        IsActive = false,
                        IsAuthenticated = false,
                        Message = $"Username {username} not found",
                        Favourites = new List<string>(),
                        Views = new List<string>(),
                        Claims = new Dictionary<string, object>()
                    };
                }

                //..map response
                var response = Mapper.Map<AuthenticationResponse>(user);
                var requestId = new IdRequest() {
                    RecordId = user.RoleId
                };
                var role = await GetRoleByIdAsync(requestId);
                if (role != null) {
                    response.RoleGroup = role.Group?.GroupCategory ?? string.Empty;
                }

                // Check if user is active
                if (!user.IsActive) {
                    Logger.LogActivity($"Inactive user attempted login: {username}", "WARN");
                    response.IsAuthenticated = false;
                    response.Message = $"User account has been deactivated";
                    return response;
                }

                // Check if user is active
                if (user.IsDeleted) {
                    Logger.LogActivity($"Deleted user attempted login: {username}", "WARN");
                    response.IsAuthenticated = false;
                    response.Message = $"User account was deleted";
                    return response;
                }

                Logger.LogActivity($"Username validation successful: {username}", "DEBUG");
                response.IsAuthenticated = false;
                response.IsAdministrator = response.RoleName.Trim() is "Administrator" or "Support";

                //..TODO update based on policy configurations
                response.CheckApproval = false;
                response.CheckVerified = false;
                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Username validation failed: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                return new AuthenticationResponse() {
                    RedirectUrl = string.Empty,
                    IsActive = false,
                    IsAuthenticated = false,
                    Message = $"Username {username} not found",
                    Favourites = new List<string>(),
                    Views = new List<string>(),
                    Claims = new Dictionary<string, object>()
                };
            }
        }

        public async Task<bool> UpdateLoginStatusAsync(long userId, DateTime loginTime) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update the logout of user with ID {userId}", "INFO");

            try {

                var user = await uow.UserRepository.GetAsync(u => u.Id == userId);
                if (user != null) {
                    //..update system users
                    user.IsLoggedIn = false;
                    user.LastLoginDate = DateTime.Now;

                    //..check entity state
                    _ = await uow.UserRepository.UpdateAsync(user);
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user role: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task UpdateLastLoginAsync(long userId, DateTime loginTime) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update user login with ID {userId}", "INFO");

            try {

                var user = await uow.UserRepository.GetAsync(u => u.Id == userId);
                if (user != null) {
                    //..update system users
                    user.IsLoggedIn = true;
                    user.LastLoginDate = loginTime;
                    user.LastModifiedOn = loginTime;
                    user.LastModifiedBy = $"{userId}";

                    //..check entity state
                    _ = await uow.UserRepository.UpdateAsync(user);
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user role: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task<bool> LogFailedLoginAsync(long userId, string ipAddress) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Log Failed Login for user with ID {userId}", "INFO");

            try {
                var attempt = new LoginAttempt {
                    UserId = userId,
                    IpAddress = ipAddress,
                    AttemptTime = DateTime.UtcNow,
                    IsSuccessful = false,
                    CreatedBy = $"{userId}",
                    CreatedOn = DateTime.Now,
                    LastModifiedOn = DateTime.Now,
                    LastModifiedBy = $"{userId}"
                };

                //..log the company data being saved
                var attemptJson = JsonSerializer.Serialize(attempt, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Attempt data: {attemptJson}", "DEBUG");

                await uow.AttemptRepository.InsertAsync(attempt);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(attempt).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                //..check if user should be locked due to too many failed attempts
                await LockUserAccountAsync(userId);
                Logger.LogActivity($"Logged failed login attempt for user: {userId} from IP: {ipAddress}");
                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"CreateCompanyAsync failed: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                //..re-throw to the controller handle
                throw;
            }
        }

        public async Task LockUserAccountAsync(long userId) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Lock user accounts for User ID {userId}", "INFO");

            try {
                //..count failed attempts in the last 15 minutes
                var cutoffTime = DateTime.UtcNow.AddMinutes(-15);

                //..get attempts
                var failedAttempts = await uow.AttemptRepository.CountAsync(u => u.UserId == userId && u.AttemptTime >= cutoffTime && !u.IsSuccessful, true);
                if (failedAttempts >= 5) {

                    var user = await uow.UserRepository.GetAsync(userId);
                    if (user != null && !user.IsActive) {
                        //..update system users
                        user.IsLoggedIn = false;
                        user.IsActive = false;
                        user.IsApproved = false;
                        user.IsVerified = false;
                        user.LastLoginDate = DateTime.Now;
                        user.LastModifiedOn = DateTime.Now;

                        //..check entity state
                        _ = await uow.UserRepository.UpdateAsync(user);
                        var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                        Logger.LogActivity($"Entity state after Update: {entityState}", "DEBUG");

                        var result = await uow.SaveChangesAsync();
                        Logger.LogActivity($"User {userId} locked due to {failedAttempts} failed login attempts", "SECURITY");
                    }

                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user role: {ex.Message}", "ERROR");

                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                throw;
            }
        }

        public async Task<WorkspaceResponse> GetWorkspaceAsync(long userId, string ipAddress) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generating user workspace for user ID {userId} at IP Address {ipAddress}", "INFO");

            WorkspaceResponse workspace = null;
            try {
                // Get all user counts sequentially
                var user = await uow.UserRepository.GetAsync(u => u.Id == userId, false, u => u.Role);
                if (user != null) {

                    //..generate workspace info
                    workspace = new WorkspaceResponse {
                        CurrentUser = new() {
                            UserId = user.Id,
                            PersonnelFileNumber = user.PFNumber,
                            Username = user.Username,
                            Email = user.EmailAddress,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        },

                        RoleId = user.RoleId,
                        Role = user.Role?.RoleName ?? string.Empty,
                    };

                    //..get brnch info
                    string solId = user.BranchSolId?.ToString();
                    if (!string.IsNullOrWhiteSpace(solId)) {
                        var branch = await uow.BranchRepository.GetAsync(b => b.SolId == solId, true, b => b.Company);
                        if (branch != null) {
                            workspace.AssignedBranch = new() {
                                BranchId = branch.Id,
                                SolId = branch.SolId,
                                BranchName = branch.BranchName,
                                OrganizationId = branch.Company?.Id ?? 0,
                                OrganizationName = branch.Company?.CompanyName ?? string.Empty,
                                OrgAlias = branch.Company?.ShortName ?? string.Empty
                            };
                        }
                    }

                    //..get prefferences
                    var preference = await uow.UserPreferenceRepository.GetAsync(u => u.UserId == user.Id);
                    if (preference != null) {
                        workspace.Preferences = new() {
                            Id = preference.Id,
                            Theme = preference.Theme,
                            Language = preference.Language
                        };
                    }

                    var views = await uow.UserViewRepository.GetAllAsync(u => u.UserId == user.Id, false);
                    if (views.Count > 0) {
                        workspace.UserViews = (from view in views select new UserViewResponse() {
                            Id = view.Id,
                            Name = view.Name,
                            View = view,
                        }).ToList();
                    }
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to process user workspace: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
            }

            return workspace;
        }

        #endregion

        #region System Users

        public bool UserExists(Expression<Func<SystemUser, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an System User exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.UserRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for System User in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> UserExistsAsync(Expression<Func<SystemUser, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an System User exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.UserRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for System User in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemUser> GetUserByEmailAsync(string email)
        {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by email", "INFO");

            try
            {

                Logger.LogActivity($"User email: {email}", "DEBUG");
                var user = await uow.UserRepository.GetAsync(u => u.EmailAddress == email);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user: {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemUser> GetByIdAsync(long id)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by ID", "INFO");

            try
            {

                Logger.LogActivity($"User ID: {id}", "DEBUG");
                var user = await uow.UserRepository.GetAsync(id);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user: {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemUser> GetUserByUsernameAsync(string username)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by email", "INFO");

            try
            {

                Logger.LogActivity($"User Username: {username}", "DEBUG");
                var user = await uow.UserRepository.GetAsync(u => u.Username == username);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user: {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<List<SystemUser>> GetAllUsersAsync()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of user records", "INFO");

            try
            {
                var users = await uow.UserRepository.GetAllAsync(true, u => u.Role, u => u.Department);
                var usersJson = JsonSerializer.Serialize(users, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                return users.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve System User records: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> InsertUserAsync(SystemUser userRecord)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save user record >>>>", "INFO");

            try
            {
                //..log the company data being saved
                var userJson = JsonSerializer.Serialize(userRecord, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"User data: {userJson}", "DEBUG");

                await uow.UserRepository.InsertAsync(userRecord);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(userRecord).State;
                Logger.LogActivity($"User state after insert: {entityState}", "DEBUG");

                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");

                return result > 0;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to insert System User record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool PasswordUpdate(PasswordResetRequest request)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Update System User request", "INFO");

            try
            {
                var user = uow.UserRepository.Get(a => a.Id == request.UserId);
                if (user != null)
                {
                    //..update System User password
                    user.PasswordHash = (request.Password ?? string.Empty).Trim();
                    user.LastModifiedOn = DateTime.Now;
                    user.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.UserRepository.Update(user);
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"System User state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update System User password: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool UpdateUser(UserRecordRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update System User request", "INFO");

            try
            {
                var user = uow.UserRepository.Get(a => a.Id == request.Id);
                if (user != null)
                {
                    //..update System User record
                    user.FirstName = (request.FirstName ?? string.Empty).Trim();
                    user.LastName = (request.LastName ?? string.Empty).Trim();
                    user.OtherName = (request.MiddleName ?? string.Empty).Trim();
                    user.Username = (request.UserName ?? string.Empty).Trim();
                    user.EmailAddress = (request.EmailAddress ?? string.Empty).Trim();
                    user.PhoneNumber = (request.PhoneNumber ?? string.Empty).Trim();
                    user.PFNumber = (request.PFNumber ?? string.Empty).Trim();
                    user.BranchSolId = (request.SolId ?? string.Empty).Trim();
                    user.DepartmentUnit = (request.UnitCode ?? string.Empty).Trim();
                    user.IsActive = request.IsActive;
                    user.IsVerified = request.IsVerified;
                    user.IsApproved = request.IsApproved;
                    user.RoleId = request.RoleId;
                    user.DepartmentId = request.DepartmentId;
                    user.IsDeleted = request.IsDeleted;
                    user.LastModifiedOn = DateTime.Now;
                    user.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.UserRepository.Update(user, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"System User state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update System User record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> UpdateUserAsync(UserRecordRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update System User", "INFO");

            try
            {
                var user = await uow.UserRepository.GetAsync(a => a.Id == request.Id);
                if (user != null)
                {
                    //..update System User record
                    user.FirstName = (request.FirstName ?? string.Empty).Trim();
                    user.LastName = (request.LastName ?? string.Empty).Trim();
                    user.OtherName = (request.MiddleName ?? string.Empty).Trim();
                    user.Username = (request.UserName ?? string.Empty).Trim();
                    user.EmailAddress = (request.EmailAddress ?? string.Empty).Trim();
                    user.PhoneNumber = (request.PhoneNumber ?? string.Empty).Trim();
                    user.PFNumber = (request.PFNumber ?? string.Empty).Trim();
                    user.BranchSolId = (request.SolId ?? string.Empty).Trim();
                    user.DepartmentUnit = (request.UnitCode ?? string.Empty).Trim();
                    user.IsActive = request.IsActive;
                    user.IsVerified = request.IsVerified;
                    user.IsApproved = request.IsApproved;
                    user.RoleId = request.RoleId;
                    user.DepartmentId = request.DepartmentId;
                    user.IsDeleted = request.IsDeleted;
                    user.LastModifiedOn = request.ModifiedOn;
                    user.LastModifiedBy = $"{request.ModifiedBy}";

                    //..check entity state
                    _ = await uow.UserRepository.UpdateAsync(user, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"System User state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update System User record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool DeleteUser(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var userJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"System User data: {userJson}", "DEBUG");

                var user = uow.UserRepository.Get(t => t.Id == request.RecordId);
                if (user != null)
                {
                    //..mark as delete this System User
                    _ = uow.UserRepository.Delete(user, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete System User : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var userJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"System User data: {userJson}", "DEBUG");

                var user = await uow.UserRepository.GetAsync(t => t.Id == request.RecordId);
                if (user != null)
                {
                    //..mark as delete this System User
                    _ = await uow.UserRepository.DeleteAsync(user, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete System User : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw;
            }
        }

        public async Task<PagedResult<SystemUser>> PagedUsersAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all System Users", "INFO");

            try
            {
                return await uow.UserRepository.PageAllAsync(pageIndex, pageSize, includeDeleted, u => u.Role, u => u.Department);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemUser>> PageAllUsersAsync(CancellationToken token, int page, int size, Expression<Func<SystemUser, bool>> predicate = null, bool includeDeleted = false)
        {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged User records", "INFO");

            try
            {
                return await uow.UserRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        #region Roles

        public bool RoleExists(Expression<Func<SystemRole, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if a System Role exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RoleRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for System Role in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> RoleExistsAsync(Expression<Func<SystemRole, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an System Role exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RoleRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for System Role in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemRole> GetRoleByIdAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve system role by ID", "INFO");

            try
            {
                Logger.LogActivity($"Role ID: {request.RecordId}", "DEBUG");
                //..get role with role group
                var role = await uow.RoleRepository.GetAsync(r => r.Id == request.RecordId, true, r => r.Group);

                //..log role record
                var roleJson = JsonSerializer.Serialize(role, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role record: {roleJson}", "DEBUG");

                return role;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system role: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemRole> GetRoleUsersAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve system role with users by ID", "INFO");
            try
            {
                Logger.LogActivity($"Role ID: {request.RecordId}", "DEBUG");
                //..get role with users
                var role = await uow.RoleRepository.GetAsync(r => r.Id == request.RecordId, true, r => r.Users);
                //..log role record
                var roleJson = JsonSerializer.Serialize(role, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role record: {roleJson}", "DEBUG");
                return role;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system role: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<string> GetRoleNameAsync(long userId)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve role name for user {userId}", "INFO");

            try
            {

                var user = await uow.UserRepository.GetAsync(u => u.Id == userId, false, x => x.Role);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role name record: {companyJson}", "DEBUG");

                return user.Role.RoleName;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve user role name: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<IList<SystemRole>> GetAllRolesAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of System Roles records", "INFO");
            try
            {
                return await uow.RoleRepository.GetAllAsync(includeDeleted, r => r.Group);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve System role records: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> InsertRoleAsync(RoleRequest request)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save role record >>>>");
            try
            {
                //..map System Role request to System Role entity
                var roleGroup = Mapper.Map<RoleRequest, SystemRole>(request);

                //..log the System Role data being saved
                var groupJson = JsonSerializer.Serialize(roleGroup, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group data: {groupJson}", "DEBUG");

                var added = await uow.RoleRepository.InsertAsync(roleGroup);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to insert System Role record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool UpdateRole(RoleRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update System Role request", "INFO");

            try
            {
                var role = uow.RoleRepository.Get(a => a.Id == request.Id);
                if (role != null)
                {
                    //..update System Role record
                    role.RoleName = (request.RoleName ?? string.Empty).Trim();
                    role.Description = (request.RoleDescription ?? string.Empty).Trim();
                    role.GroupId = request.GroupId;
                    role.IsVerified = request.IsVerified;
                    role.IsApproved = request.IsApproved;
                    role.IsDeleted = request.IsDeleted;
                    role.LastModifiedOn = DateTime.Now;
                    role.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.RoleRepository.Update(role, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(role).State;
                    Logger.LogActivity($"System Role state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update System Role record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> UpdateRoleAsync(RoleRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update System Role", "INFO");

            try
            {
                var role = await uow.RoleRepository.GetAsync(a => a.Id == request.Id, includeDeleted);
                if (role != null) {
                    //..update System Role record
                    role.RoleName = (request.RoleName ?? string.Empty).Trim();
                    role.Description = (request.RoleDescription ?? string.Empty).Trim();
                    role.GroupId = request.GroupId;
                    role.IsVerified = request.IsVerified;
                    role.IsApproved = request.IsApproved;
                    role.IsDeleted = request.IsDeleted;
                    role.LastModifiedOn = DateTime.Now;
                    role.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.RoleRepository.UpdateAsync(role, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(role).State;
                    Logger.LogActivity($"System Role state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update System Role record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool DeleteRole(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var roleJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"System Role data: {roleJson}", "DEBUG");

                var user = uow.UserRepository.Get(t => t.Id == request.RecordId);
                if (user != null)
                {
                    //..mark as delete this System Role
                    _ = uow.UserRepository.Delete(user, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete System Role : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var roleJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"System Role data: {roleJson}", "DEBUG");

                var role = await uow.RoleRepository.GetAsync(t => t.Id == request.RecordId);
                if (role != null)
                {
                    //..mark as delete this System Role
                    _ = await uow.RoleRepository.DeleteAsync(role, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(role).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Role Group : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw;
            }
        }

        public async Task<PagedResult<SystemRole>> PagedRolesAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all System Role", "INFO");

            try {
                return await uow.RoleRepository.PageAllAsync(pageIndex, pageSize, includeDeleted, r => r.Group);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve System Role records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemRole>> PageAllRolesAsync(CancellationToken token, int page, int size, Expression<Func<SystemRole, bool>> predicate = null, bool includeDeleted = false)
        {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged System Role records", "INFO");

            try
            {
                return await uow.RoleRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve System Role records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        #region Role Groups

        public bool RoleGroupExists(Expression<Func<SystemRoleGroup, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Role Group exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RoleGroupRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Role Group in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> RoleGroupExistsAsync(Expression<Func<SystemRoleGroup, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Role Group exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RoleGroupRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Role Group in the database: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemRoleGroup> GetRoleGroupByIdAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve system role group by ID", "INFO");

            try
            {
                Logger.LogActivity($"Role Group ID: {request.RecordId}", "DEBUG");
                //..get role group
                var role = await uow.RoleGroupRepository.GetAsync(r => r.Id == request.RecordId, true, r => r.Roles);

                //..log role group record
                var roleJson = JsonSerializer.Serialize(role, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group record: {roleJson}", "DEBUG");

                return role;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system role group: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<IList<SystemRoleGroup>> GetAllRoleGroupsAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of Role group records", "INFO");
            try
            {
                return await uow.RoleGroupRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role group records: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> InsertRoleGroupAsync(RoleGroupRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save role group record >>>>");
            try
            {
                //..map Role Group request to Role Group entity
                var roleGroup = Mapper.Map<RoleGroupRequest, SystemRoleGroup>(request);

                //..log the Role Group data being saved
                var groupJson = JsonSerializer.Serialize(roleGroup, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group data: {groupJson}", "DEBUG");

                var added = await uow.RoleGroupRepository.InsertAsync(roleGroup);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to insert Role Group record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> InsertRoleGroupWithRolesAsync(RoleGroupRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save role group record >>>>");
            try
            {
                //..map Role Group request to Role Group entity
                var roleGroup = Mapper.Map<RoleGroupRequest, SystemRoleGroup>(request);

                //..log the Role Group data being saved
                var groupJson = JsonSerializer.Serialize(roleGroup, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group data: {groupJson}", "DEBUG");

                if(request.Roles != null && request.Roles.Count > 0) {
                    roleGroup.Roles = new List<SystemRole>();
                    foreach (var roleId in request.Roles) {
                        var role = await uow.RoleRepository.GetAsync(r => r.Id == roleId);
                        if (role != null)
                        {
                            roleGroup.Roles.Add(role);
                        }
                    }
                }

                var added = await uow.RoleGroupRepository.InsertAsync(roleGroup);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to insert Role Group record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> InsertRoleGroupWithPermissionSetsAsync(RoleGroupRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save role group record >>>>");
            try {
                //..map Role Group request to Role Group entity
                var roleGroup = Mapper.Map<RoleGroupRequest, SystemRoleGroup>(request);

                //..log the Role Group data being saved
                var groupJson = JsonSerializer.Serialize(roleGroup, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group data: {groupJson}", "DEBUG");

                if (request.PermissionSets != null && request.PermissionSets.Count > 0) {
                    roleGroup.PermissionSets = new List<SystemRoleGroupPermissionSet>();
                    foreach (var psId in request.PermissionSets) {
                        var permSet = await uow.PermissionSetRepository.GetAsync(r => r.Id == psId);
                        if (permSet != null) {
                            roleGroup.PermissionSets.Add(new SystemRoleGroupPermissionSet { 
                                RoleGroup = roleGroup,
                                PermissionSet = permSet
                            });
                        }
                    }
                }

                var added = await uow.RoleGroupRepository.InsertAsync(roleGroup);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to insert Role Group record: {ex.Message}", "ERROR");

                //..log inner exceptions here too
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool UpdateRoleGroup(RoleGroupRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Role Group request", "INFO");

            try
            {
                var roleGroup = uow.RoleGroupRepository.Get(a => a.Id == request.Id);
                if (roleGroup != null)
                {
                    //..update Role Group record
                    roleGroup.GroupName = (request.GroupName ?? string.Empty).Trim();
                    roleGroup.Description = (request.GroupDescription ?? string.Empty).Trim();
                    roleGroup.GroupCategory = (request.GroupCategory?? "ADMINSUPPORT").Trim();
                    roleGroup.Scope = (GroupScope)request.GroupScope;
                    roleGroup.Type = (RoleGroup)request.Type;
                    roleGroup.Department = (request.DepartmentName ?? string.Empty).Trim();
                    roleGroup.IsVerified = request.IsVerified;
                    roleGroup.IsApproved = request.IsApproved;
                    roleGroup.IsDeleted = request.IsDeleted;
                    roleGroup.LastModifiedOn = DateTime.Now;
                    roleGroup.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.RoleGroupRepository.Update(roleGroup, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Role Group state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Role Group record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> UpdateRoleGroupAsync(RoleGroupRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Role Group", "INFO");

            try
            {
                var roleGroup = await uow.RoleGroupRepository.GetAsync(a => a.Id == request.Id);
                if (roleGroup != null)
                {
                    //..update Role Group record
                    roleGroup.GroupName = (request.GroupName ?? string.Empty).Trim();
                    roleGroup.Description = (request.GroupDescription ?? string.Empty).Trim();
                    roleGroup.GroupCategory = (request.GroupCategory ?? string.Empty).Trim();
                    roleGroup.Scope = (GroupScope)request.GroupScope;
                    roleGroup.Type = (RoleGroup)request.Type;
                    roleGroup.Department = (request.DepartmentName ?? string.Empty).Trim();
                    roleGroup.IsVerified = request.IsVerified;
                    roleGroup.IsApproved = request.IsApproved;
                    roleGroup.IsDeleted = request.IsDeleted;
                    roleGroup.LastModifiedOn = DateTime.Now;
                    roleGroup.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.RoleGroupRepository.UpdateAsync(roleGroup, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Role Group state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Role Group record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool DeleteRoleGroup(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var roleGroupJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group data: {roleGroupJson}", "DEBUG");

                var user = uow.RoleGroupRepository.Get(t => t.Id == request.RecordId);
                if (user != null)
                {
                    //..mark as delete this Role Group
                    _ = uow.RoleGroupRepository.Delete(user, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(user).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Role Group : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> DeleteRoleGroupAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var roleGroupJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Role Group data: {roleGroupJson}", "DEBUG");

                var roleGroup = await uow.RoleGroupRepository.GetAsync(t => t.Id == request.RecordId);
                if (roleGroup != null)
                {
                    //..mark as delete this Role Group
                    _ = await uow.RoleGroupRepository.DeleteAsync(roleGroup, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(roleGroup).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Role Group : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                throw;
            }
        }

        public async Task<PagedResult<SystemRoleGroup>> PagedRoleGroupsAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all Role Groups", "INFO");

            try
            {
                return await uow.RoleGroupRepository.PageAllAsync(pageIndex, pageSize, includeDeleted, g => g.Roles);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Role Group records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemRoleGroup>> PageAllRoleGroupsAsync(CancellationToken token, int page, int size, Expression<Func<SystemRoleGroup, bool>> predicate = null, bool includeDeleted = false)
        {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Role Group records", "INFO");

            try
            {
                return await uow.RoleGroupRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Role Group records : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<SystemRoleGroup> GetRoleGroupWithPermissionSetsByIdAsync(IdRequest request) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Role Group records", "INFO");

            try
            {
                var roleGroup = await uow.RoleGroupRepository.GetAsync(ps => ps.Id == request.RecordId, request.markAsDeleted, p => p.PermissionSets);

                if (roleGroup == null)
                    return null;

                // Ensure unique permission sets
                roleGroup.PermissionSets = roleGroup.PermissionSets
                    .GroupBy(p => p.PermissionSetId)
                    .Select(g => g.First())
                    .ToList();

                return roleGroup;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Role Group records : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemRoleGroup>> PagedRoleGroupWithPermissionSetsAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Role Group records", "INFO");

            try
            {
                return await uow.RoleGroupRepository.PageAllAsync(pageIndex, pageSize, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Role Group records : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        #region System Permissions

        public async Task<List<SystemPermission>> GetAllPermissionsAsync()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of system permissions", "INFO");
            try
            {
                return (await uow.PermissionRepository.GetAllAsync(true)).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system permissions: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<List<SystemPermissionSet>> GetRolePermissionSetsAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of Role permission sets", "INFO");

            try {
                //..load the Role with its Group and their PermissionSets
                var roles = await uow.RoleRepository.GetAllAsync(
                    r => r.Id == request.RecordId,
                    request.markAsDeleted,
                    r => r.Group,
                    r => r.PermissionSets,
                    r => r.Group.PermissionSets
                );

                var role = roles.FirstOrDefault();
                if (role == null)
                    return new List<SystemPermissionSet>();

                //..collect role-specific permission sets
                var rolePermissionSets = role.PermissionSets
                    .Select(rp => rp.PermissionSet)
                    .Where(ps => ps != null);

                //..collect group-level inherited permission sets
                var groupPermissionSets = role.Group?.PermissionSets?
                    .Select(gp => gp.PermissionSet)
                    .Where(ps => ps != null)
                    ?? Enumerable.Empty<SystemPermissionSet>();

                //..combine and remove duplicates
                var allPermissionSets = rolePermissionSets
                    .Concat(groupPermissionSets)
                    .DistinctBy(ps => ps.Id)
                    .ToList();

                return allPermissionSets;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role permission sets: {ex.Message}", "ERROR");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company?.Id ?? 1;

                SystemError errorObj = new()
                {
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message,
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<List<SystemPermission>> GetRolePermissionsAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of Role permissions", "INFO");

            try {
                var roles = await uow.RoleRepository.GetAllAsync(
                    r => r.Id == request.RecordId,
                    request.markAsDeleted,
                    r => r.Group,
                    r => r.PermissionSets,
                    r => r.Group.PermissionSets
                );

                var role = roles.FirstOrDefault();
                if (role == null)
                    return new List<SystemPermission>();

                //..collect all permission sets
                var rolePermissionSets = role.PermissionSets
                    .Select(rp => rp.PermissionSet)
                    .Where(ps => ps != null);

                var groupPermissionSets = role.Group?.PermissionSets?
                    .Select(gp => gp.PermissionSet)
                    .Where(ps => ps != null)
                    ?? Enumerable.Empty<SystemPermissionSet>();

                var allPermissionSets = rolePermissionSets
                    .Concat(groupPermissionSets)
                    .DistinctBy(ps => ps.Id)
                    .ToList();

                foreach (var ps in allPermissionSets) {
                    await uow.Context.Entry(ps)
                        .Collection(p => p.Permissions)
                        .Query()
                        .Include(pp => pp.Permission)
                        .LoadAsync();
                }

                var allPermissions = allPermissionSets
                    .SelectMany(ps => ps.Permissions)
                    .Select(pp => pp.Permission)
                    .DistinctBy(p => p.Id)
                    .ToList();

                return allPermissions;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve role permissions: {ex.Message}", "ERROR");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company?.Id ?? 1;

                SystemError errorObj = new()
                {
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message,
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }
        }

        public async Task<bool> UpdateRolePermissionSetsAsync(long roleId, List<long> newPermissionSetIds) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Merging permission sets for Role {roleId}", "INFO");

            try
            {
                var role = await uow.RoleRepository.GetAsync(
                    r => r.Id == roleId,
                    false,
                    r => r.PermissionSets
                ) ?? throw new Exception($"Role with ID {roleId} not found");
                var existingIds = role.PermissionSets.Select(p => p.PermissionSetId).ToList();
                var (toAdd, toRemove) = GetChanges(existingIds, newPermissionSetIds);

                //..remove outdated role-permission-set links
                role.PermissionSets = role.PermissionSets
                    .Where(p => !toRemove.Contains(p.PermissionSetId))
                    .ToList();

                //..add new links
                foreach (var setId in toAdd)
                {
                    role.PermissionSets.Add(new SystemRolePermissionSet
                    {
                        RoleId = role.Id,
                        PermissionSetId = setId
                    });
                }

                await uow.RoleRepository.UpdateAsync(role);
                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                return result > 0;

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to merge permission sets for Role : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<List<SystemPermission>> GetPermissionSetPermissionsAsync(long permissionSetId, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of permissions for a permission set", "INFO");

            try
            {
                var sets = await uow.PermissionSetRepository.GetAllAsync(ps => ps.Id == permissionSetId, includeDeleted, ps => ps.Permissions.Select(pps => pps.Permission));
                var set = sets.FirstOrDefault();
                if (set == null)
                    return new List<SystemPermission>();

                var permissions = set.Permissions
                    .Select(pps => pps.Permission)
                    .DistinctBy(p => p.Id)
                    .ToList();

                return permissions;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve set permissions: {ex.Message}", "ERROR");

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }

                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
                long companyId = company?.Id ?? 1;

                SystemError errorObj = new()
                {
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message,
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };

                await uow.SystemErrorRespository.InsertAsync(errorObj);
                throw;
            }

        }

        public async Task<bool> UpdatePermissionSetPermissionsAsync(long permissionSetId, List<long> newPermissionIds) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Merging permissions for PermissionSet {permissionSetId}", "INFO");

            try
            {
                var set = await uow.PermissionSetRepository.GetAsync(
                    ps => ps.Id == permissionSetId, false,
                    ps => ps.Permissions
                ) ?? throw new Exception($"PermissionSet with ID {permissionSetId} not found");
                var existingIds = set.Permissions.Select(p => p.PermissionId).ToList();
                var (toAdd, toRemove) = GetChanges(existingIds, newPermissionIds);

                // Remove outdated links
                set.Permissions = set.Permissions
                    .Where(p => !toRemove.Contains(p.PermissionId))
                    .ToList();

                // Add new links
                foreach (var permId in toAdd)
                {
                    set.Permissions.Add(new SystemPermissionPermissionSet
                    {
                        PermissionId = permId,
                        PermissionSetId = set.Id
                    });
                }

                await uow.PermissionSetRepository.UpdateAsync(set);
                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                return result > 0;

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to merge permissions for PermissionSet : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemPermission>> PagedPermissionsAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all system permissions", "INFO");

            try
            {
                return await uow.PermissionRepository.PageAllAsync(pageIndex, pageSize, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system permissions records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemPermission>> PageAllPermissionsAsync(CancellationToken token, int page, int size, Expression<Func<SystemPermission, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all system permissions", "INFO");

            try
            {
                return await uow.PermissionRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system permissions records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        #region System Permission Sets

        public async Task<SystemPermissionSet> GetPermissionSetAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieving PermissionSet {request.RecordId} with permissions", "INFO");

            try {
                var sets = await uow.PermissionSetRepository.GetAllAsync(
                    ps => ps.Id == request.RecordId,
                    true,
                    query => query
                        .Include(ps => ps.Permissions)
                            .ThenInclude(psp => psp.Permission)
                );

                var set = sets.FirstOrDefault();
                if (set == null)
                    return null;

                //..ensure permissions are distinct
                set.Permissions = set.Permissions
                    .DistinctBy(p => p.PermissionId) 
                    .ToList();

                return set;
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PermissionSetResponse> GetPermissionSetWithAssignedPermissionsAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieving PermissionSet {request.RecordId} with all permissions", "INFO");

            try
            {
                // Load the permission set with join table
                var sets = await uow.PermissionSetRepository.GetAllAsync(
                    ps => ps.Id == request.RecordId,
                    request.markAsDeleted,
                    ps => ps.Permissions
                );

                var set = sets.FirstOrDefault();

                if (set == null)
                    return null;

                // Get IDs of permissions assigned to this set
                var assignedPermissionIds = set.Permissions
                    .Select(psp => psp.PermissionId)
                    .Distinct()
                    .ToHashSet(); // HashSet for O(1) lookup

                // Load ALL permissions
                var allPermissions = await uow.PermissionRepository.GetAllAsync(
                    p => true,
                    request.markAsDeleted
                );

                //..map to response
                var response = new PermissionSetResponse
                {
                    Id = set.Id,
                    SetName = set.SetName,
                    CreatedOn = set.CreatedOn,
                    CreatedBy = set.CreatedBy ?? string.Empty,
                    ModifiedOn = set.LastModifiedOn,
                    ModifiedBy = set.LastModifiedBy ?? string.Empty,
                    Permissions = allPermissions
                        .Select(p => new PermissionResponse
                        {
                            Id = p.Id,
                            PermissionName = p.PermissionName,
                            PermissionDescription = p.Description,
                            IsAssigned = assignedPermissionIds.Contains(p.Id)
                        })
                        .OrderByDescending(p => p.IsAssigned) 
                        .ThenBy(p => p.PermissionName) 
                        .ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> PermissionSetExistsAsync(Expression<Func<SystemPermissionSet, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Permission Set exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.PermissionSetRepository.ExistsAsync(predicate, false);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<List<SystemPermissionSet>> GetRoleGroupPermissionSetsAsync(long roleGroupId, bool includePermissions = false) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieving permission sets for RoleGroup {roleGroupId}", "INFO");

            try {
                var groups = await uow.RoleGroupRepository.GetAllAsync(
                g => g.Id == roleGroupId,
                false,
                g => g.PermissionSets.Select(rgp => rgp.PermissionSet), includePermissions ?
                g => g.PermissionSets.Select(rgp => rgp.PermissionSet.Permissions.Select(p => p.Permission)):
                null);

                var group = groups.FirstOrDefault();
                if (group == null)
                    return new List<SystemPermissionSet>();

                return group.PermissionSets
                            .Select(rgp => rgp.PermissionSet)
                            .DistinctBy(ps => ps.Id)
                            .ToList();
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> UpdateRoleGroupPermissionSetsAsync(long roleGroupId, List<long> newPermissionSetIds) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Merging permission sets for RoleGroup {roleGroupId}", "INFO");
            try
            {
                var group = await uow.RoleGroupRepository.GetAsync(
                    g => g.Id == roleGroupId,false,
                    g => g.PermissionSets
                ) ?? throw new Exception($"RoleGroup with ID {roleGroupId} not found");
                var existingIds = group.PermissionSets.Select(p => p.PermissionSetId).ToList();
                var (toAdd, toRemove) = GetChanges(existingIds, newPermissionSetIds);

                //..remove outdated links
                group.PermissionSets = group.PermissionSets
                    .Where(p => !toRemove.Contains(p.PermissionSetId))
                    .ToList();

                //..add new links
                foreach (var setId in toAdd) {
                    group.PermissionSets.Add(new SystemRoleGroupPermissionSet {
                        RoleGroupId = group.Id,
                        PermissionSetId = setId
                    });
                }

                await uow.RoleGroupRepository.UpdateAsync(group);
                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                return result > 0;

            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> InsertPermissionSetAsync(PermissionSetRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save Permission Set record >>>>");
            try
            {
                //..map Permission Set request to Permission Set entity
                var permissionSet = new SystemPermissionSet
                {
                    SetName = (request.SetName ?? string.Empty).Trim(),
                    Description = (request.Description ?? string.Empty).Trim(),
                    IsDeleted = false,
                    CreatedBy = $"{request.UserId}",
                    CreatedOn = DateTime.Now,
                    Permissions = new List<SystemPermissionPermissionSet>()
                };

                // Link permissions if provided
                if (request.Permissions != null && request.Permissions.Any())
                {
                    var permissions = await uow.PermissionRepository.GetAllAsync(p => request.Permissions.Contains(p.Id));
                    foreach (var perm in permissions)
                    {
                        permissionSet.Permissions.Add(new SystemPermissionPermissionSet {
                            Permission = perm,
                            PermissionSet = permissionSet
                        });
                    }
                }


                //..log the Permission Set data being saved
                var setJson = JsonSerializer.Serialize(permissionSet, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Permission Set data: {setJson}", "DEBUG");

                var added = await uow.PermissionSetRepository.InsertAsync(permissionSet);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(permissionSet).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool UpdatePermissionSet(PermissionSetRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Permission Set", "INFO");

            try
            {
                var permissionSet = uow.PermissionSetRepository.Get(a => a.Id == request.Id);
                if (permissionSet != null)
                {
                    //..update Permission Set record
                    permissionSet.SetName = (request.SetName ?? string.Empty).Trim();
                    permissionSet.Description = (request.Description ?? string.Empty).Trim();
                    permissionSet.IsDeleted = request.IsDeleted;
                    permissionSet.LastModifiedOn = DateTime.Now;
                    permissionSet.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.PermissionSetRepository.Update(permissionSet, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(permissionSet).State;
                    Logger.LogActivity($"Permission Set state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Permission Set record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<bool> UpdatePermissionSetAsync(PermissionSetRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Permission Set", "INFO");

            try
            {
                var permissionSet = await uow.PermissionSetRepository.GetAsync(a => a.Id == request.Id, true, s => s.Permissions) 
                    ?? throw new Exception($"Permission Set with ID {request.Id} not found");
                if (permissionSet != null) {
                    //..update Permission Set record
                    permissionSet.SetName = (request.SetName ?? string.Empty).Trim();
                    permissionSet.Description = (request.Description ?? string.Empty).Trim();
                    permissionSet.IsDeleted = request.IsDeleted;
                    permissionSet.LastModifiedOn = DateTime.Now;
                    permissionSet.LastModifiedBy = $"{request.UserId}";

                    //..get permissions
                    var newPermissionsIds = request.Permissions;
                    var existingIds = permissionSet.Permissions.Select(p => p.PermissionId).ToList();
                    var (toAdd, toRemove) = GetChanges(existingIds, newPermissionsIds);

                    //..remove outdated permission-set-permission links
                    permissionSet.Permissions = permissionSet.Permissions.Where(p => !toRemove.Contains(p.PermissionId)).ToList();

                    //..add new links
                    foreach (var permissionId in toAdd) {
                        permissionSet.Permissions.Add(new SystemPermissionPermissionSet {
                            PermissionId = permissionId, 
                            PermissionSetId = permissionSet.Id
                        });
                    }

                    //..check entity state
                    _ = await uow.PermissionSetRepository.UpdateAsync(permissionSet, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(permissionSet).State;
                    Logger.LogActivity($"Permission Set state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Permission Set record: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public bool DeletePermissionSet(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var permissionSetJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Permission Set data: {permissionSetJson}", "DEBUG");

                var set = uow.PermissionSetRepository.Get(ps => ps.Id == request.RecordId, includeDeleted: true, ps => ps.Permissions);
                if (set == null) {
                    Logger.LogActivity($"PermissionSet {request.RecordId} not found", "WARN");
                    return false;
                }

                if (request.markAsDeleted) {
                    // Soft delete
                    set.IsDeleted = true;
                    set.LastModifiedOn = DateTime.UtcNow;
                    set.LastModifiedBy = $"{request.UserId}";
                    uow.PermissionSetRepository.Update(set);
                    Logger.LogActivity($"PermissionSet {set.SetName} marked as deleted.", "INFO");
                }
                else
                {
                    // Remove join table entries first
                    set.Permissions.Clear();
                    uow.PermissionSetRepository.Delete(set);
                    Logger.LogActivity($"PermissionSet {set.SetName} permanently deleted.", "INFO");

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(set).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                }

                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                return result > 0;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Permission Set : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<bool> DeletePermissionSetAsync(IdRequest request){
            using var uow = UowFactory.Create();
            try
            {
                var permissionSetJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Permission Set data: {permissionSetJson}", "DEBUG");

                var set = await uow.PermissionSetRepository.GetAsync(ps => ps.Id == request.RecordId, includeDeleted: true, ps => ps.Permissions);
                if (set == null)
                {
                    Logger.LogActivity($"PermissionSet {request.RecordId} not found", "WARN");
                    return false;
                }

                if (request.markAsDeleted)
                {
                    // Soft delete
                    set.IsDeleted = true;
                    set.LastModifiedOn = DateTime.UtcNow;
                    set.LastModifiedBy = $"{request.UserId}";
                    await uow.PermissionSetRepository.UpdateAsync(set);
                    Logger.LogActivity($"PermissionSet {set.SetName} marked as deleted.", "INFO");
                }
                else
                {
                    // Remove join table entries first
                    set.Permissions.Clear();
                    await uow.PermissionSetRepository.DeleteAsync(set);
                    Logger.LogActivity($"PermissionSet {set.SetName} permanently deleted.", "INFO");

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(set).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                }

                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                return result > 0;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Permission Set : {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
                    StackTrace = ex.StackTrace,
                    Severity = "CRITICAL",
                    ReportedOn = DateTime.Now,
                    CompanyId = companyId
                };
                throw;
            }
        }

        public async Task<List<SystemPermissionSet>> GetAllPermissionSetsAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve list of Permission Sets", "INFO");
            try
            {
                return (await uow.PermissionSetRepository.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve permission sets records: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemPermissionSet>> PagedPermissionSetAsync(int page = 1, int size = 10, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all system permission sets", "INFO");

            try
            {
                return await uow.PermissionSetRepository.PageAllAsync(page, size, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system permissions records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<SystemPermissionSet>> PageAllPermissionSetAsync(CancellationToken token, int page, int size, Expression<Func<SystemPermissionSet, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all system permission sets", "INFO");

            try
            {
                return await uow.PermissionSetRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve system permission sets records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        #region System Activities

        public async Task<ActivityLog> GetActivityLogAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve activity log by ID", "INFO");

            try {

                Logger.LogActivity($"Activity Log ID: {request.RecordId}", "DEBUG");

                //..get activity log
                var role = await uow.ActivityLogRepository.GetAsync(a => a.Id == request.RecordId, true, a => a.ActivityType, a => a.User);

                //..log activity log record
                var roleJson = JsonSerializer.Serialize(role, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Activity Log record: {roleJson}", "DEBUG");

                return role;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve activity log: {ex.Message}", "ERROR");

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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        public async Task<PagedResult<ActivityLog>> GetPagedActivityLogAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve all activity logs", "INFO");

            try
            {
                return await uow.ActivityLogRepository.PageAllAsync(pageIndex, pageSize, includeDeleted, a => a.ActivityType, a => a.User);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve activity logs records : {ex.Message}", "ERROR");
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
                    ErrorSource = "SYSTEM-ACCESS-SERVICE",
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

        private async Task LogErrorAsync(IUnitOfWork uow, Exception ex)
        {
            Logger.LogActivity($"Failed to retrieve permission set: {ex.Message}", "ERROR");

            var currentEx = ex.InnerException;
            while (currentEx != null)
            {
                Logger.LogActivity($"Service Inner Exception: {currentEx.Message}", "ERROR");
                currentEx = currentEx.InnerException;
            }

            // Get company ID efficiently
            var company = await uow.CompanyRepository.GetAllAsync(c => true, false);
            long companyId = company.FirstOrDefault()?.Id ?? 1L;

            // Get innermost exception
            var innermostException = ex;
            while (innermostException.InnerException != null)
                innermostException = innermostException.InnerException;

            var errorObj = new SystemError
            {
                ErrorMessage = innermostException.Message,
                ErrorSource = "SYSTEM-ACCESS-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            uow.SystemErrorRespository.Insert(errorObj);
            await uow.SaveChangesAsync();
        }

        private static (List<long> toAdd, List<long> toRemove) GetChanges(IEnumerable<long> existingIds, IEnumerable<long> newIds) {
            var existingSet = new HashSet<long>(existingIds);
            var newSet = new HashSet<long>(newIds);

            var toAdd = newSet.Except(existingSet).ToList();
            var toRemove = existingSet.Except(newSet).ToList();

            return (toAdd, toRemove);
        }

        #endregion

    }
}
