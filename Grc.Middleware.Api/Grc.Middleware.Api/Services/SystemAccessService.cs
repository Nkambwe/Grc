using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Services {

    public class SystemAccessService : BaseService, ISystemAccessService {

        public SystemAccessService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory unitOfWorkFactory)
            : base(loggerFactory, unitOfWorkFactory) {
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
    }
}
