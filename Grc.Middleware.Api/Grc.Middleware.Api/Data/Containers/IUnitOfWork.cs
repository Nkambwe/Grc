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
        IResponsibilityRepository ResponsibilityRepository { get; set; }
        IRegulatoryReturnRepository RegulatoryReturnRepository { get; set; }
        IReturnTypeRepository ReturnTypeRepository { get; set; }
        IRegulatoryTypeRepository RegulatoryTypeRepository { get; set; }
        IGuideDocumentRepository GuideDocumentRepository { get; set; }
        IGuideDocumentTypeRepository GuideDocumentTypeRepository { get; set; }
        ICircularSubmissionRepository CircularSubmissionRepository { get; set; }
        IProcessTagRepository ProcessTagRepository { get; set; }
        IProcessTaskRepository ProcessTaskRepository { get; set; }
        IProcessActivityRepository ProcessActivityRepository { get; set; }
        IProcessGroupRepository ProcessGroupRepository { get; set; }
        IProcessTypeRepository ProcessTypeRepository { get; set; }
        IOperationProcessRepository OperationProcessRepository { get; set; }
        IAuditExceptionRepository AuditExceptionRepository { get; set; }
        IAuditReportRepository AuditReportRepository { get; set; }
        IAuditRepository AuditRepository { get; set; }
        IAuditTaskRepository AuditTaskRepository { get; set; }

        Task<int> SaveChangesAsync(); 
        int SaveChanges(); 
    }

}
