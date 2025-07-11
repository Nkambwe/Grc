using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface ICompanyFactory {
         Task<CompanyRegistrationModel> PrepareCompanyRegistrationModelAsync();
    }
}
