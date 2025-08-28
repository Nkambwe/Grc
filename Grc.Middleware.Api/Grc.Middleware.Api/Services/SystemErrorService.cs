using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Grc.Middleware.Api.Services {
    public class SystemErrorService : BaseService, ISystemErrorService {
        public SystemErrorService(IServiceLoggerFactory loggerFactory, 
            IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<BugCountResponse> GetErrorCountsAsync(long companyId) {
             using var uow = UowFactory.Create();
             Logger.LogActivity($"Admin Dashboard bug Statistics", "INFO");
            try {
                
                // Get all bug counts sequentially
                var totalBugs = await uow.SystemErrorRespository.CountAsync();
                var newBugs = await uow.SystemErrorRespository.CountAsync(b => b.FixStatus == "OPEN"); 
                
                return new BugCountResponse {
                    TotalBugs = totalBugs,
                    NewBugs = newBugs
                };
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve admin bug dashboard statistics: {ex.Message}", "ERROR");
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
    
        }

        public async Task<bool> SaveErrorAsync(SystemError errorObj) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Save system error record >>>>", "INFO");
    
            try {
                //..log the error data being saved
                var errorJson = JsonSerializer.Serialize(errorObj, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"System Error data: {errorJson}", "DEBUG");
                await uow.SystemErrorRespository.InsertAsync(errorObj);

                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(errorObj).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
        
                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"SaveErrorAsync failed: {ex.Message}", "ERROR");
        
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
    }
}
