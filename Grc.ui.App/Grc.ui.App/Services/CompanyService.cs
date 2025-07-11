using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {

    public class CompanyService : GrcBaseService, ICompanyService {

        public CompanyService(IApplicationLoggerFactory loggerFactory, 
                              IHttpClientFactory httpClientFactory,
                              IEnvironmentProvider environment, 
                              IEndpointTypeProvider endpointType)
            : base(loggerFactory, httpClientFactory, environment,endpointType) {
            Logger.Channel = $"COMPANY-{DateTime.Now:yyyyMMddHHmmss}";
        }

    }

}
