using AutoMapper;
using Grc.ui.App.Utils;
using System.Text.Json;

namespace Grc.ui.App.Services {

    public class GrcBaseService : IGrcBaseService {
        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IEndpointTypeProvider EndpointProvider;
        protected readonly JsonSerializerOptions JsonOptions;
        protected readonly IMapper Mapper;
        protected readonly IHttpHandler HttpHandler;
        public GrcBaseService(IApplicationLoggerFactory loggerFactory, 
                              IHttpHandler httpHandler,
                              IEnvironmentProvider environment,
                              IEndpointTypeProvider endpointType,
                              IMapper mapper) {
            Logger = loggerFactory.CreateLogger("app_services");
            Environment = environment;
            EndpointProvider = endpointType;
            HttpHandler = httpHandler;
            Mapper = mapper;
            JsonOptions = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }
    }
}
