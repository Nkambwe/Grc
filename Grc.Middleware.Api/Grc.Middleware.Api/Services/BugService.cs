using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {
    public class BugService : BaseService, IBugService {
        public BugService(IServiceLoggerFactory loggerFactory, 
                            IUnitOfWorkFactory uowFactory, 
                            IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<PagedResult<BugItemResponse>> GetBugsAsync(BugListRequest request) {
            return await Task.FromResult(new PagedResult<BugItemResponse>(){ 
                    Entities = new List<BugItemResponse>(),
                    Page = 0,
                    Count = 0,
                    Size = 0,
                
                });
        }
    }
}
