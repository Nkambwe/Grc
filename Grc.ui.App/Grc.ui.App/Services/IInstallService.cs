using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Services {

    public interface IInstallService : IGrcBaseService {
        /// <summary>
        /// Register company
        /// </summary>
        /// <param name="model">Company model to register</param>
        /// <returns>Service response object</returns>
        Task<GrcResponse<ServiceResponse>> RegisterCompanyAsync(CompanyRegistrationModel model);
    }
}
