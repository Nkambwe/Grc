using AutoMapper;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Infrastructure.MvcHelpers;
using Grc.ui.App.Routes;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Grc.ui.App.Extensions.Mvc {

    public static class ServiceExtension {

        /// <summary>
        /// Application logging
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection RegisterServices(this IServiceCollection services) {

            //register routes
            services.AddSingleton<IRouteProvider, RouteProvider>();
            services.AddSingleton<IRoutePublisher, RoutePublisher>();

            //..register service
            services.AddScoped<IMiddlewareHealthService, MiddlewareHealthService>();
            services.AddScoped<IGrcHtml, GrcHtml>();
            services.AddScoped<IGRCFileProvider, GRCFileProvider>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IGrcBaseService, GrcBaseService>();
            services.AddScoped<ISystemAccessService, SystemAccessService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            
            
            //..register factories 
            services.AddScoped<IRegistrationFactory, CompanyFactory>();
            
            //..sessionExtensions manager
            services.AddScoped<SessionManager>();

            //..allow html helpers to acces current action context
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IWebHelper, WebHelper>();
            return services;
        }

        /// <summary>
        /// Auto Mapper Configurations
        /// </summary>
        /// <param name="services">Service Collection</param>
        public static void ObjectMapper(this IServiceCollection services) {

            var mappingConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

     }

}
