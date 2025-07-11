using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class CompanyFactory : ICompanyFactory {

        protected readonly ICompanyService _orgService;

        public CompanyFactory(ICompanyService orgService) { 
            _orgService = orgService;
        }

        public Task<CompanyRegistrationModel> PrepareCompanyRegistrationModelAsync()
            => Task.FromResult(new CompanyRegistrationModel());
    }
}
