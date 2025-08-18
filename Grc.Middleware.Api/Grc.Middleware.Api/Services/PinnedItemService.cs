using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Grc.Middleware.Api.Services {
    public class PinnedItemService : BaseService, IPinnedItemService {
        public PinnedItemService(IServiceLoggerFactory loggerFactory, 
                                 IUnitOfWorkFactory uowFactory, 
                                 IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<IList<PinnedItemResponse>> GetUserPinnedItemsAsync(long recordId) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get a list of all user pinned items", "INFO");
            try {

                var items = await uow.PinnedItemRepository.GetAllAsync(i => i.UserId == recordId, false);
                IList<PinnedItemResponse> pins = new List<PinnedItemResponse>();
                if (items != null && items.Count > 0) { 
                    foreach(var item in items) { 
                        pins.Add(Mapper.Map<PinnedItemResponse>(item));
                    }
                }
                //..log pinned record
                var pinsJson = JsonSerializer.Serialize(pins, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Pinned item records: {pinsJson}", "DEBUG");

                return pins;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user pinned items: {ex.Message}", "ERROR");
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
