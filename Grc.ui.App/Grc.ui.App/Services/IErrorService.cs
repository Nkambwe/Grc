using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IErrorService {
        /// <summary>
        /// Save a system error and notify
        /// </summary>
        /// <param name="error">Error object to save</param>
        Task<GrcResponse<ServiceResponse>>  SaveSystemErrorAsync(GrcErrorModel error, string ipAddress);
    }
}
