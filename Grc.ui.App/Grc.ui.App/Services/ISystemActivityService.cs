using AutoMapper;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {

    public interface ISystemActivityService : IGrcBaseService {

        Task<GrcResponse<PagedResponse<ActivityModel>>> GetActivityLogsAsync(TableListRequest request);
        Task<GrcResponse<ServiceResponse>> InsertActivityAsync(long userId, string activity, string comment, string systemKeyword=null, string entityName=null, string ipAddress = null);
    }

}
