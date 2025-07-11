using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Extensions.Mvc;
using Grc.ui.App.Http.Endpoints;
using Grc.ui.App.Middleware;
using Grc.ui.App.Routes;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Grc.ui.App {

    public class Startup {
        private readonly IConfiguration _configuration;
    
        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }
    
        /// <summary>
        /// Service configuration
        /// </summary>
        /// <param name="services">Service Interface</param>
        public void ConfigureServices(IServiceCollection services) {
            //..register appSettings variable options
            services.Configure<EnvironmentOptions>(_configuration.GetSection(EnvironmentOptions.SectionName));
            services.Configure<LoggingOptions>(_configuration.GetSection(LoggingOptions.SectionName));
            services.Configure<MiddlewareOptions>(_configuration.GetSection(MiddlewareOptions.SectionName));
            services.Configure<EndpointTypeOptions>(_configuration.GetSection(EndpointTypeOptions.SectionName));
        
            //..register appSettings variable providers
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddScoped<IEndpointTypeProvider, EndpointTypeProvider>();
        
            //..register logger factory
            services.AddScoped<IApplicationLoggerFactory, ApplicationLoggerFactory>();
        
            //..register auto mapper
            services.ObjectMapper();
        
            // Add middleware client
            services.AddApiClients(_configuration);
        
            //..register application session - FIXED: Use consistent timeout value
            var sessionOptions = _configuration.GetSection("MiddlewareOptions").Get<MiddlewareOptions>();
            int timeOut = sessionOptions?.IdleSessionTime ?? 30;
            services.AddApplicationSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromMinutes(timeOut);
                options.Cookie.Name = ".Grc.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        
            //..configure HttpClient with base URL
            var middlewareOptions = _configuration.GetSection("MiddlewareOptions").Get<MiddlewareOptions>();
            services.AddHttpClient("MiddlewareClient", client => {
                if (!string.IsNullOrEmpty(middlewareOptions?.BaseUrl)) {
                    client.BaseAddress = new Uri(middlewareOptions.BaseUrl.TrimEnd('/') + '/');
                }
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        
            //..register services
            services.RegisterServices();
        
            //..add MVC
            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        
            //..add logging 
            services.AddLogging(builder => {
                builder.AddConsole();
                builder.AddDebug();
            });
        }
        
        /// <summary>
        /// Configure application builder
        /// </summary>
        /// <param name="app">Application buildr instance</param>
        /// <returns></returns>
        public void Configure(WebApplication app) { 
            //..debug service registration
            using (var scope = app.Services.CreateScope()) {
                ServiceRegistrationDebugger.LogRegisteredServices(scope.ServiceProvider);
            }
        
            //.use appSettings environment variable directly
            var envOptions = _configuration.GetSection("EnvironmentOptions").Get<EnvironmentOptions>();
            if (!(bool)envOptions?.IsLive) {
                app.UseExceptionHandler("/Error/Status404");
                app.UseHsts();
            }
        
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            //..add session
            app.UseSession();

            //..add authorization
            app.UseAuthorization();
        
            //..add session middleware
            var middlewareOptions = _configuration.GetSection("MiddlewareOptions").Get<MiddlewareOptions>();
            int appSessionTime = middlewareOptions?.AppSessionTime ?? 45;
            app.UseApplicationSession(TimeSpan.FromMinutes(appSessionTime));
        
            //..register middleware health monitor
            app.UseMiddleware<HealthCheckMiddleware>();
        
            //..register routes
            var routePublisher = app.Services.GetRequiredService<IRoutePublisher>();
            routePublisher.RegisterRoutes(app);
        }
    
    }
}
