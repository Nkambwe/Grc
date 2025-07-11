using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Data.Repositories;

namespace Grc.Middleware.Api.Data.Containers {

    public interface IUnitOfWork : IDisposable {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        ICompanyRepository CompanyRepository { get; }
        Task<int> SaveChangesAsync(); 
        int SaveChanges(); 
    }

}
