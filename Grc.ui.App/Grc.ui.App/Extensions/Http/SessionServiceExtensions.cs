using Grc.ui.App.Utils;

namespace Grc.ui.App.Extensions.Http {

    /// <summary>
    /// extension method to register session
    /// </summary>
    public static class SessionServiceExtensions {

        public static IServiceCollection AddApplicationSession(this IServiceCollection services, Action<SessionOptions> configureSession = null) { 
            //..configure HttpContext accessor
            services.AddHttpContextAccessor();
        
            //..configure session, distributed memory cache is required for session storage
            services.AddDistributedMemoryCache();
        
            if (configureSession == null) {
                services.AddSession(options => { 
                    options.IdleTimeout = TimeSpan.FromHours(2);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            } else { 
                services.AddSession(configureSession);
            }
        
            //..register SessionManager
            services.AddScoped<SessionManager>();
        
            return services;
        }

    }
}
