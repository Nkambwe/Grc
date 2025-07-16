using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IRegistrationFactory {
         Task<CompanyRegistrationModel> PrepareCompanyRegistrationModelAsync();
        Task<NoServiceModel> PrepareNoServiceModelAsync();
    }
}
