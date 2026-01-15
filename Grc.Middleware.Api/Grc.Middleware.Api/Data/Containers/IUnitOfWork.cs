using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Data.Repositories;

namespace Grc.Middleware.Api.Data.Containers {

    public interface IUnitOfWork : IDisposable {
        public GrcContext Context { get;} 
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        ICompanyRepository CompanyRepository { get; set;}
        IBranchRepository BranchRepository { get; set;}
        IUserRepository UserRepository { get; set;}
        IRoleRepository RoleRepository { get; set; }
        IRoleGroupRepository RoleGroupRepository { get; set; }
        IUserViewRepository UserViewRepository{get; set;}
        IUserPreferenceRepository UserPreferenceRepository { get; set;}
        IAttemptRepository AttemptRepository { get; set; }
        IQuickActionRepository QuickActionRepository { get; set; }
        IPinnedItemRepository PinnedItemRepository { get; set; }
        ISystemErrorRespository SystemErrorRespository { get; set; }
        IActivityTypeRepository ActivityTypeRepository { get; set; }
        IActivityLogRepository ActivityLogRepository { get; set; }
        IActivityLogSettingRepository ActivityLogSettingRepository { get; set; }
        IDepartmentRepository DepartmentRepository { get; set; }
        IDepartmentUnitRepository DepartmentUnitRepository { get; set; }
        IAuthoritiesRepository AuthoritiesRepository { get; set; }
        IFrequencyRepository FrequencyRepository { get; set; }
        IRegulatoryCategoryRepository RegulatoryCategoryRepository { get; set; }
        IStatutoryRegulationRepository StatutoryRegulationRepository { get; set; }
        IStatutoryArticleRepository StatutoryArticleRepository { get; set; }
        IArticleRevisionRepository ArticleRevisionRepository { get; set; }
        IControlCategoryRepository ControlCategoryRepository { get; set; }
        IControlItemRepository ControlItemRepository { get; set; }
        IComplianceIssueRepository ComplianceIssueRepository { get; set; }
        IResponsebilityRepository ResponsebilityRepository { get; set; }
        IReturnRepository ReturnRepository { get; set; }
        ICircularRepository CircularRepository { get; set; }
        ICircularIssueRepository CircularIssueRepository { get; set; }
        IReturnTypeRepository ReturnTypeRepository { get; set; }
        IRegulatoryTypeRepository RegulatoryTypeRepository { get; set; }
        IRegulatoryDocumentRepository RegulatoryDocumentRepository { get; set; }
        IRegulatoryDocumentTypeRepository RegulatoryDocumentTypeRepository { get; set; }
        IRegulatorySubmissionRepository RegulatorySubmissionRepository { get; set; }
        IProcessTagRepository ProcessTagRepository { get; set; }
        IProcessTaskRepository ProcessTaskRepository { get; set; }
        IProcessActivityRepository ProcessActivityRepository { get; set; }
        IProcessGroupRepository ProcessGroupRepository { get; set; }
        IProcessTypeRepository ProcessTypeRepository { get; set; }
        IOperationProcessRepository OperationProcessRepository { get; set; }
        IProcessApprovalRepository ProcessApprovalRepository { get; set; }
        IAuditExceptionRepository AuditExceptionRepository { get; set; }
        IAuditReportRepository AuditReportRepository { get; set; }
        IAuditRepository AuditRepository { get; set; }
        IAuditTaskRepository AuditTaskRepository { get; set; }
        IAuditTypeRepository AuditTypeRepository { get; set; }
        IAuditUpdateRepository AuditUpdateRepository { get; set; }
        INotificationRepository NotificationRepository { get; set; }
        ISystemConfigurationRepository SystemConfigurationRepository { get; set; }
        IPermissionRepository PermissionRepository { get; set; }
        IPermissionSetRepository PermissionSetRepository { get; set; }
        IMailSettingsRepository MailSettingsRepository { get; set; }
        IMailRecordRepository MailRecordRepository { get; set; }
        Task<int> SaveChangesAsync(); 
        int SaveChanges(); 
    }

}
