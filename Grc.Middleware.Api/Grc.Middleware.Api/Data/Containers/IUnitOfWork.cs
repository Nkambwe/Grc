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

        Task<int> SaveChangesAsync(); 
        int SaveChanges(); 
    }

}
