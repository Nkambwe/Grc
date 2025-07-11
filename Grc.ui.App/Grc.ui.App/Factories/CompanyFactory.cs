using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class CompanyFactory : IRegistrationFactory {

        protected readonly ICompanyService _orgService;

        public CompanyFactory(ICompanyService orgService) { 
            _orgService = orgService;
        }

        public Task<CompanyRegistrationModel> PrepareCompanyRegistrationModelAsync() {
            var company = new CompanyRegistrationModel(){
                CompanyName = string.Empty,
                Alias = string.Empty,
                RegistrationNumber = string.Empty
            };
            return Task.FromResult(company);
        }
    }
}
