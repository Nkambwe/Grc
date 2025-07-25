﻿using AutoMapper;
using Grc.Middleware.Api.Data;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Repositories;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Extensions {

    public static class ServiceExtension {

        /// <summary>
        /// Register repositories
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void RegisterRepositories(this IServiceCollection services) {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICompanyRepository, CompanyRepository>();
        }

        /// <summary>
        /// Register UnitOfWork
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void RegisterUnitOfWork(this IServiceCollection services) {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        }

        /// <summary>
        /// Register Middleware services
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void RegisterServices(this IServiceCollection services) {

            //..register service
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<ICompanyService, CompanyService>();
            

            //allow html helpers to acces current action context
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        /// <summary>
        /// Configure AutoMapper
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void ObjectMapper(this IServiceCollection services) {

            var mappingConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        /// <summary>
        /// Configure Cors
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void ConfigureCors(this IServiceCollection services) {
            services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", builder => {
                    builder.WithOrigins("10.30.039")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        /// <summary>
        /// Configure Forward Headers
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void ConfigureForwardHeaders(this IServiceCollection services) {
            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                // Add known proxies
                options.KnownProxies.Add(System.Net.IPAddress.Parse("127.0.0.1"));

                // trust all networks (NOTE: use only in trusted environments)
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        /// <summary>
        /// Database connection configuration
        /// </summary>
        /// <param name="services">Service instance</param>
        public static void ConfigureDatabaseConnection(this IServiceCollection services, IConfiguration Configuration) {
            //..create logger
            using var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetRequiredService<IServiceLoggerFactory>();
            var _logger = loggerFactory.CreateLogger("grc_middleware_log");
            _logger.Channel = $"DBCONNECTION-{DateTime.Now:yyyyMMddHHmmss}";
            _logger.LogActivity("Attempting DB Connection...", "GRC_CONFIG");
            try {

                //..connection variable name
                var connectionVar = Configuration.GetValue<string>("ConnectionOptions:DefaultConnection");

                if (!string.IsNullOrWhiteSpace(connectionVar)) {
                    //..get appSettings environment variable directly
                    var isLive = Configuration.GetValue<bool>("EnvironmentOptions:IsLive");
                    services.AddDbContextFactory<GrcContext>(options => {

                        //Retrieve the connection string from environment variables
                        string connectionString = Environment.GetEnvironmentVariable(connectionVar);

                        if (!string.IsNullOrEmpty(connectionString)) {
                            string decryptedString = HashGenerator.DecryptString(connectionString);

                            if (isLive) {
                                _logger.LogActivity($"CONNECTION URL :: {connectionString}", "INFO");
                            } else {
                                _logger.LogActivity($"CONNECTION URL :: {decryptedString}", "INFO");
                            }

                            options.UseSqlServer(decryptedString);
                            _logger.LogActivity("Data Connection Established", "GRC_CONFIG");
                        } else {
                            string msg = "Environmental variable name 'GRC_DBCONNECTION_ENV' which holds connection string not found";
                            _logger.LogActivity(msg, "DB_ERROR");
                            throw new Exception(msg);
                        }

                    });
                    _logger.LogActivity($"DB Connection Established...", "GRC_CONFIG");
                } else {
                    string msg = "DB Connection Environment variable name 'GRC_DBCONNECTION' not found in appSettings";
                    _logger.LogActivity(msg, "DB_ERROR");
                    throw new Exception(msg);
                }

            } catch (Exception e) {
                string msg = "Database connection error occurred";
                _logger.LogActivity(msg, "GRC_CONFIG");
                _logger.LogActivity($" {e.Message}", "DB_ERROR");
                _logger.LogActivity($" {e.StackTrace}", "STACKTRACE");

                throw new Exception(msg);
            }
        }

    }
}
