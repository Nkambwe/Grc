using Grc.ui.App.Defaults;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Extensions.Mvc;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Http.Endpoints;
using Grc.ui.App.Infrastructure.MvcHelpers;
using Grc.ui.App.Menus;
using Grc.ui.App.Middleware;
using Grc.ui.App.Routes;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

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
            services.Configure<LanguageOptions>(_configuration.GetSection(LanguageOptions.SectionName));
        
            //..register appSettings variable providers
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddScoped<IEndpointTypeProvider, EndpointTypeProvider>();

            //..register factories 
            services.AddScoped<IApplicationLoggerFactory, ApplicationLoggerFactory>();
            services.AddScoped<IErrorFactory, ErrorFactory>();
            services.AddScoped<ILoginFactory, LoginFactory>();
            services.AddScoped<IRegistrationFactory, RegistrationFactory>();
            services.AddScoped<IReceientModelFactory, ReceientModelFactory>();
            services.AddScoped<IPinnedModelFactory, PinnedModelFactory>();
            services.AddScoped<IQuickActionModelFactory, QuickActionModelFactory>();
            services.AddScoped<IDashboardFactory, DashboardFactory>();
            services.AddScoped<IPageModelFactory, PageModelFactory>();
            services.AddScoped<IDepartmentFactory, DepartmentFactory>();
            services.AddScoped<ISupportDashboardFactory, SupportDashboardFactory>();
            services.AddScoped<IGrcErrorFactory, GrcErrorFactory>();
            services.AddScoped<IOperationsDashboardFactory, OperationsDashboardFactory>(); 
            services.AddScoped<IPolicyRegisterFactory, PolicyRegisterFactory>(); 
            services.AddScoped<IStatuteFactory, StatuteFactory>(); 
            services.AddScoped<IControlFactory, ControlFactory>();
            services.AddScoped<IConfigurationFactory, ConfigurationFactory>();

            //..register session manager
            services.AddScoped<SessionManager>();
            services.AddSingleton<ISupportMenuRegistry, SupportMenuRegistry>();
            services.AddSingleton<IOperationsMenuRegistry, OperationsMenuRegistry>();

            //..register auto mapper
            services.ObjectMapper();
        
            // Add middleware client
            services.AddApiClients(_configuration);
        
            //..register application session 
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

            //..add anti-forgery services
            services.AddAntiforgery(options => {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "__RequestVerificationToken";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            services.AddScoped<GrcAntiForgeryTokenAttribute>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.Cookie.Name = CookieDefaults.UserCookie;
                options.LoginPath = "/login/userlogin";
                options.LogoutPath = "/app/logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });
            services.AddAuthorization();
        
            //..configure HttpClient middleware client
            var middlewareOptions = _configuration.GetSection("MiddlewareOptions").Get<MiddlewareOptions>();
            services.AddHttpClient("MiddlewareClient", client => {
                var envOptions = _configuration.GetSection("EnvironmentOptions").Get<EnvironmentOptions>();
                
                if (!(bool)envOptions?.IsLive) {
                    //..Configure uat environment
                    if (!string.IsNullOrEmpty(middlewareOptions?.BaseUrl)) {
                        client.BaseAddress = new Uri(middlewareOptions.BaseUrl.TrimEnd('/') + '/');
                    }
                } else {
                     //..Configure production environment
                    if (!string.IsNullOrEmpty(middlewareOptions?.ProdBaseUrl)) {
                        client.BaseAddress = new Uri(middlewareOptions.ProdBaseUrl.TrimEnd('/') + '/');
                    }
                }
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        
            //..register services
            services.RegisterServices();
            
            //..register filters
            services.AddScoped<RecentMenuItemAttribute>();

            //..add MVC
            services.AddControllersWithViews(options => {
                options.Filters.AddService<RecentMenuItemAttribute>();
            });

            //..configure razor pages options
            services.Configure<RazorViewEngineOptions>(options => {
                options.ViewLocationFormats.Add("/Views/Shared/TagHelpers/{0}.cshtml");
            });

            //..register taghelper
            services.AddScoped<GrcAntiForgeryTokenTagHelper>();

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

            //..get localizaed languages
            var LanguageOptions = _configuration.GetSection("LanguageOptions").Get<LanguageOptions>();
            var supportedCultures = LanguageOptions?.SupportedCultures ?? Array.Empty<string>();
            var localizationOptions = new RequestLocalizationOptions()
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture(LanguageOptions?.DefaultCulture?? CommonDefaults.DefaultLanguageCulture);

            //..add cookie provider for localization
            localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            app.UseRequestLocalization(localizationOptions);

            //..register routes
            var routePublisher = app.Services.GetRequiredService<IRoutePublisher>();
            routePublisher.RegisterRoutes(app);
        }
    
    }
}
