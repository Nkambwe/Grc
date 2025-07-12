using Grc.ui.App.Http.Endpoints;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.Extensions.Options;

namespace Grc.ui.App.Extensions.Mvc {

    /// <summary>
    /// Debugging class to check successfully loadded services
    /// </summary>
    public static class ServiceRegistrationDebugger {
        public static void LogRegisteredServices(IServiceProvider serviceProvider) {
            Console.WriteLine("=== SERVICE REGISTRATION DEBUG ===");

            try {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                Console.WriteLine("IHttpClientFactory registered");
            } catch (Exception ex) {
                Console.WriteLine($"IHttpClientFactory not registered: {ex.Message}");
            }
            
            try {
                var loggingOptions = serviceProvider.GetRequiredService<IOptions<LoggingOptions>>();
                Console.WriteLine($"LoggingOptions - Log File Name   : {loggingOptions.Value.DefaultLogName}");
                Console.WriteLine($"LoggingOptions - Log Folder Name : {loggingOptions.Value.LogFolder}");
            } catch (Exception ex) {
                Console.WriteLine($"EndpointType not registered: {ex.Message}");
            }

            try {
                var middlewareOptions = serviceProvider.GetRequiredService<IOptions<MiddlewareOptions>>();
                Console.WriteLine($"MiddlewareOptions registered - BaseUrl: {middlewareOptions.Value.BaseUrl}");
            } catch (Exception ex) {
                Console.WriteLine($"MiddlewareOptions not registered: {ex.Message}");
            }
            
            try {
                var langOptions = serviceProvider.GetRequiredService<IOptions<LanguageOptions>>();
                Console.WriteLine($"LanguageOptions registered - Default Culture: {langOptions.Value.DefaultCulture}");
                Console.WriteLine($"Supported Cultures : {langOptions.Value.SupportedCultures}");
            } catch (Exception ex) {
                Console.WriteLine($"LanguageOptions not registered: {ex.Message}");
            }

            try {
                var endpointOptions = serviceProvider.GetRequiredService<IOptions<EndpointTypeOptions>>();
                Console.WriteLine($"EndpointType registered - Health.Status: {endpointOptions.Value.Health.Status}");
            } catch (Exception ex) {
                Console.WriteLine($"EndpointType not registered: {ex.Message}");
            }
        
            try {
                var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();
                Console.WriteLine($"IEnvironmentProvider registered - IsLive: {environmentProvider.IsLive}");
            } catch (Exception ex) {
                Console.WriteLine($"IEnvironmentProvider not registered: {ex.Message}");
            }
        
            try {
                var loggerFactory = serviceProvider.GetRequiredService<IApplicationLoggerFactory>();
                Console.WriteLine("IApplicationLoggerFactory registered");
            } catch (Exception ex) {
                Console.WriteLine($"IApplicationLoggerFactory not registered: {ex.Message}");
            }

            try {
                var healthService = serviceProvider.GetRequiredService<IMiddlewareHealthService>();
                Console.WriteLine("IMiddlewareHealthService registered");
            } catch (Exception ex) {
                Console.WriteLine($"IMiddlewareHealthService not registered: {ex.Message}");
            }
        
            Console.WriteLine("=== END SERVICE REGISTRATION DEBUG ===");
        }
    }
}
