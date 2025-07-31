using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Grc.Middleware.Api.Http.Responses;
using AutoMapper;
using System.Security.Claims;
using Azure;

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
            Logger.LogActivity("Retrieve user by email", "INFO");
    
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
    }
}
