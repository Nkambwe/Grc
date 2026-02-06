using AutoMapper;
using Grc.Middleware.Api.Data;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Repositories;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Compliance.Audits;
using Grc.Middleware.Api.Services.Compliance.Regulations;
using Grc.Middleware.Api.Services.Compliance.Support;
using Grc.Middleware.Api.Services.Operations;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.TaskHandler;
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
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentUnitRepository, DepartmentUnitRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();
            services.AddScoped<IUserViewRepository, UserViewRepository>();
            services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
            services.AddScoped<IAttemptRepository, AttemptRepository>();
            services.AddScoped<IQuickActionRepository, QuickActionRepository>();
            services.AddScoped<IPinnedItemRepository, PinnedItemRepository>();
            services.AddScoped<ISystemErrorRespository, SystemErrorRespository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<ISystemConfigurationRepository, SystemConfigurationRepository>();
            services.AddScoped<IActivityTypeRepository, ActivityTypeRepository>();
            services.AddScoped<IActivityLogSettingRepository, ActivityLogSettingRepository>();
            services.AddScoped<IAuthoritiesRepository, AuthoritiesRepository>();
            services.AddScoped<IFrequencyRepository, FrequencyRepository>();
            services.AddScoped<IRegulatoryCategoryRepository, RegulatoryCategoryRepository>();
            services.AddScoped<IReturnRepository, ReturnRepository>();
            services.AddScoped<ICircularRepository, CircularRepository>();
            services.AddScoped<ICircularIssueRepository, CircularIssueRepository>();
            services.AddScoped<IRegulatoryTypeRepository, RegulatoryTypeRepository>();
            services.AddScoped<IReturnTypeRepository, ReturnTypeRepository>();
            services.AddScoped<IResponsebilityRepository, ResponsebilityRepository>();
            services.AddScoped<IStatutoryArticleRepository, StatutoryArticleRepository>();
            services.AddScoped<IArticleRevisionRepository, ArticleRevisionRepository>();
            services.AddScoped<IStatutoryRegulationRepository, StatutoryRegulationRepository>();
            services.AddScoped<IControlCategoryRepository, ControlCategoryRepository>();
            services.AddScoped<IControlItemRepository, ControlItemRepository>();
            services.AddScoped<IComplianceIssueRepository, ComplianceIssueRepository>();
            services.AddScoped<IRegulatoryDocumentRepository, GuideDocumentRepository>();
            services.AddScoped<IRegulatoryDocumentTypeRepository, GuideDocumentTypeRepository>();
            services.AddScoped<IRegulatorySubmissionRepository, CircularSubmissionRepository>();
            services.AddScoped<IProcessTagRepository, ProcessTagRepository>();
            services.AddScoped<IProcessTaskRepository, ProcessTaskRepository>();
            services.AddScoped<IProcessActivityRepository, ProcessActivityRepository>();
            services.AddScoped<IProcessGroupRepository, ProcessGroupRepository>();
            services.AddScoped<IProcessTypeRepository, ProcessTypeRepository>();
            services.AddScoped<IProcessApprovalRepository, ProcessApprovalRepository>();
            services.AddScoped<IOperationProcessRepository, OperationProcessRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionSetRepository, PermissionSetRepository>();
            services.AddScoped<IAuditExceptionRepository, AuditExceptionRepository>();
            services.AddScoped<IAuditReportRepository, AuditReportRepository>();
            services.AddScoped<IAuditUpdateRepository, AuditUpdateRepository>();
            services.AddScoped<IAuditTypeRepository, AuditTypeRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>(); 
            services.AddScoped<IAuditTaskRepository, AuditTaskRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMailSettingsRepository, MailSettingsRepository>();
            services.AddScoped<IMailRecordRepository, MailRecordRepository>();
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
            services.AddScoped<IErrorNotificationService, ErrorNotificationService>();
            services.AddScoped<ISystemErrorService, SystemErrorService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ISystemAccessService, SystemAccessService>();
            services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddScoped<IActivityTypeService, ActivityTypeService>();
            services.AddScoped<IActivityLogSettingService, ActivityLogSettingService>();
            services.AddScoped<IDepartmentsService, DepartmentsService>();
            services.AddScoped<IDepartmentUnitService, DepartmentUnitService>();
            services.AddScoped<IQuickActionService, QuickActionService>();
            services.AddScoped<IPinnedItemService, PinnedItemService>();
            services.AddScoped<IAuthorityService, AuthorityService>(); 
            services.AddScoped<IFrequencyService, FrequencyService>();
            services.AddScoped<IRegulatoryCategoryService, RegulatoryCategoryService>();
            services.AddScoped<IReturnService, ReturnService>();
            services.AddScoped<IRegulatoryTypeService, RegulatoryTypeService>();
            services.AddScoped<IResponsebilityService, ResponsebilityService>();
            services.AddScoped<IReturnTypeService, ReturnTypeService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStatutoryArticleService, StatutoryArticleService>();
            services.AddScoped<IArticleRevisionService, ArticleRevisionService>();
            services.AddScoped<IStatutoryRegulationService, StatutoryRegulationService>();
            services.AddScoped<IComplianceIssueService, ComplianceIssueService>();
            services.AddScoped<IControlItemService, ControlItemService>();
            services.AddScoped<IControlCategoryService, ControlCategoryService>();
            services.AddScoped<IRegulatoryDocumentService, RegulatoryDocumentService>();
            services.AddScoped<IRegulatoryDocumentTypeService, RegulatoryDocumentTypeService>();
            services.AddScoped<IReturnsSubmissionService, ReturnsSubmissionService>();
            services.AddScoped<IProcessTagService, ProcessTagService>();
            services.AddScoped<IProcessTaskService, ProcessTaskService>();
            services.AddScoped<IProcessActivityService, ProcessActivityService>();
            services.AddScoped<IProcessGroupService, ProcessGroupService>();
            services.AddScoped<IProcessTypeService, ProcessTypeService>();
            services.AddScoped<IOperationProcessService, OperationProcessService>();
            services.AddScoped<IProcessApprovalService, ProcessApprovalService>();
            services.AddScoped<ICircularService, CircularService>();
            services.AddScoped<ICircularIssueService, CircularIssueService>();
            services.AddScoped<IAuditExceptionService, AuditExceptionService>();
            services.AddScoped<IAuditReportService, AuditReportService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IAuditTaskService, AuditTaskService>();
            services.AddScoped<IAuditTypeService, AuditTypeService>();
            services.AddScoped<IAuditUpdateService, AuditUpdateService>();
            services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
            services.AddScoped<IBugService, BugService>();
            services.AddScoped<IMailService, MailService>();
            services.AddSingleton<IMailTaskQueue, MailTaskQueue>();
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
                    builder.WithOrigins("10.129.2.39")
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
            var _logger = loggerFactory.CreateLogger("middleware_log");
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
