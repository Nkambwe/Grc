using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Grc.Middleware.Api.Http.Responses;
using AutoMapper;

namespace Grc.Middleware.Api.Services {

    public class SystemAccessService : BaseService, ISystemAccessService {

        public SystemAccessService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
            : base(loggerFactory, unitOfWorkFactory, mapper) {
        }

        public async Task<SystemUser> GetByEmailAsync(string email) {
            
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by email", "INFO");
    
            try {

                Logger.LogActivity($"User email: {email}", "DEBUG");
                var user = await uow.UserRepository.GetAsync(u => u.EmailAddress == email);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }

        public async Task<SystemUser> GetByIdAsync(long id) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by ID", "INFO");
    
            try {

                Logger.LogActivity($"User ID: {id}", "DEBUG");
                var user = await uow.UserRepository.GetAsync(id);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }

        public async Task<SystemUser> GetByUsernameAsync(string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve user by email", "INFO");
    
            try {

                Logger.LogActivity($"User Username: {username}", "DEBUG");
                var user = await uow.UserRepository.GetAsync(u => u.Username == username);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw; 
            }
        }

        public async Task<string> GetUserRoleAsync(long userId) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve user role for user {userId}", "INFO");
    
            try {

                var user = await uow.UserRepository.GetAsync(u => u.Id == userId, false, x => x.Role);

                //..log user record
                var companyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"User record: {companyJson}", "DEBUG");

                return user.Role.RoleName;
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

        public async Task<int> GetTotalUsersCountAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Total User Count", "INFO");
    
            try {
                return await uow.UserRepository.CountAsync();
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
                return await uow.UserRepository.CountAsync(u => u.IsActive);
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
                    return new AuthenticationResponse(){ 
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
        
                return new AuthenticationResponse(){ 
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

        public async  Task<bool> UpdateLoginStatusAsync(long userId, DateTime loginTime) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update the logout of user with ID {userId}", "INFO");
    
            try {

                var user = await uow.UserRepository.GetAsync(u => u.Id == userId);
                if(user != null){ 
                    //..update system users
                    user.IsLoggedIn = false;
                    user.LastLoginDate = DateTime.Now;
                   
                    //..check entity state
                    _= await uow.UserRepository.UpdateAsync(user);
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
                if(user != null){ 
                    //..update system users
                    user.IsLoggedIn = true;
                    user.LastLoginDate = loginTime;
                    user.LastModifiedOn = loginTime;
                    user.LastModifiedBy = $"{userId}";
                   
                    //..check entity state
                    _= await uow.UserRepository.UpdateAsync(user);
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
                var failedAttempts  = await uow.AttemptRepository.CountAsync(u => u.Id == userId && u.AttemptTime >= cutoffTime && !u.IsSuccessful);
                 if (failedAttempts >= 5){ 

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

    }
}
