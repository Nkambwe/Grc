using AutoMapper;
using Grc.ui.App.Helpers;
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

            //..register health service
            services.AddScoped<IMiddlewareHealthService, MiddlewareHealthService>();
            services.AddScoped<IHttpHandler, HttpHandler>();
            services.AddScoped<IGrcBaseService, GrcBaseService>();
            services.AddScoped<IGRCFileProvider, GRCFileProvider>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ISystemAccessService, SystemAccessService>();
            services.AddScoped<IInstallService, InstallService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPinnedService, PinnedService>();
            services.AddScoped<IQuickActionService, QuickActionService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDepartmentUnitService, DepartmentUnitService>();
            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<ISystemActivityService, SystemActivityService>();
            services.AddScoped<IProcessesService, ProcessesService>(); 
            services.AddScoped<IReturnsService, ReturnsService>();
            services.AddScoped<IRegulatonCategoryService, RegulatonCategoryService>(); 
            services.AddScoped<IRegulatonTypeService, RegulatonTypeService>();
            services.AddScoped<IRegulatonAuthorityService, RegulatonAuthorityService>();
            services.AddScoped<IAuditService, AuditService>(); 
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddScoped<IPolicyTaskService, PolicyTaskService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>(); 
            services.AddScoped<IResponsibilityService, ResponsibilityService>();
            services.AddScoped<IStatutorySectionService, StatutorySectionService>(); 
            services.AddScoped<IRegulatoryStatuteService, RegulatoryStatuteService>();
            services.AddScoped<IComplianceControlService, ComplianceControlService>();
            services.AddScoped<ISystemConfiguration, SystemConfiguration>();

            //..sessionExtensions manager
            services.AddScoped<SessionManager>();

            //..allow html helpers to acces current action context
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IGrcHtml, GrcHtml>();
            services.AddScoped<IWebHelper, WebHelper>();
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
