using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Grc.Middleware.Api.Services {

    public class QuickActionService : BaseService, IQuickActionService {
        public QuickActionService(IServiceLoggerFactory loggerFactory, 
                                  IUnitOfWorkFactory uowFactory, 
                                  IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<IList<QuickActionResponse>> GetUserQuickActionAsync(long recordId) {
           using var uow = UowFactory.Create();
            Logger.LogActivity("Get a list of all user quick action", "INFO");
            try {

                var items = await uow.QuickActionRepository.GetAllAsync(q => q.UserId == recordId, false);
                IList<QuickActionResponse> actions = new List<QuickActionResponse>();
                if (items != null && items.Count > 0) { 
                    foreach(var item in items) { 
                        actions.Add(Mapper.Map<QuickActionResponse>(item));
                    }
                }
                //..log quick action records
                var actionJson = JsonSerializer.Serialize(actions, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Quick Actions record: {actionJson}", "DEBUG");

                return actions;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user quick actions: {ex.Message}", "ERROR");
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
