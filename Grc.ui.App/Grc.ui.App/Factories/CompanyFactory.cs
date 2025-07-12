using Grc.ui.App.Defaults;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class CompanyFactory : IRegistrationFactory {

        private readonly ICompanyService _orgService;
        private readonly IHttpContextAccessor _httpContext;

        public CompanyFactory(IHttpContextAccessor httpContext, ICompanyService orgService) { 
            _orgService = orgService;
            _httpContext = httpContext;
        }

        public Task<CompanyRegistrationModel> PrepareCompanyRegistrationModelAsync() {
            var httpContext = _httpContext.HttpContext;
            var languageCode = $"{CookieDefaults.Prefix}{CookieDefaults.LanguageCookie}";
            var languageFromCookie = httpContext.Request.Cookies[languageCode];

            CompanyRegistrationModel company = new(){
                CompanyName = string.Empty,
                Alias = string.Empty,
                RegistrationNumber = string.Empty,
                SystemLanguage = languageFromCookie ?? "None",
                SystemAdmin = new()
            };
            return Task.FromResult(company);
        }
    }
}
