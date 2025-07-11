using Microsoft.AspNetCore.HttpOverrides;
using Grc.Middleware.Api.Utils;
using Grc.Middleware.Api.Extensions;

namespace Grc.Middleware.Api {

    public class Startup {

         public IConfiguration Configuration { get; }

         public Startup(IConfiguration configuration) { 
            Configuration = configuration;
         }

        /// <summary>
        /// Servervice configuration
        /// </summary>
        /// <param name="services">Service collection instance</param>
        public void ConfigureServices(IServiceCollection services) {

            //..get appSettings settings
            services.Configure<EnvironmentOptions>(Configuration.GetSection(EnvironmentOptions.SectionName));
            services.Configure<LoggingOptions>(Configuration.GetSection(LoggingOptions.SectionName));
            services.Configure<UrlOptions>(Configuration.GetSection(UrlOptions.SectionName));
            services.Configure<DataConnectionOptions>(Configuration.GetSection(DataConnectionOptions.SectionName));
            
            //..register appSettings provider
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddScoped<IServiceLoggerFactory, ServiceLoggerFactory>();
            services.AddScoped<IDataConnectionProvider, DataConnectionProvider>();
            services.AddScoped<IUrlProvider, UrlProvider>();
 
            // database connection
            services.ConfigureDatabaseConnection(Configuration);
            
            //register UnitOfWork
            services.RegisterUnitOfWork();

            //register Repositories
            services.RegisterRepositories();

            //..register services
            services.RegisterServices();
            
            //Mvc configurations
            services.AddRazorPages();
            services.AddEndpointsApiExplorer();
            services.AddControllers().AddJsonOptions(options => {
                //keep JSON Property names as they are
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //format JSON data to make it more readable
                options.JsonSerializerOptions.WriteIndented = true;
            });

            // swagger
            services.AddSwaggerGen(c => {
                c.CustomSchemaIds(type => type.FullName);
            });
 
            //..register auto mapper
            services.ObjectMapper();

            // cross origin configuration
            services.ConfigureCors();
 
            //forward headers
            services.ConfigureForwardHeaders();
 
            // http configurations
            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Configure application builder
        /// </summary>
        /// <param name="app">Web Application instance</param>
        public void Configure(WebApplication app) {

            //..use appSettings environment variable directly
            var isLive = Configuration.GetValue<bool>("EnvironmentOptions:IsLive");
            if (!isLive) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
 
            if (!isLive) {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
 
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
 
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

        }
    }
}
