﻿using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Http.Requests;

namespace Grc.Middleware.Api.Services {

    public interface IBranchService : IBaseService { 
        Task<Branch> GetByIdAsync(long id, bool includeDeleted=false);
        Task<Branch> GetByNameAsync(string name, bool includeDeleted=false);
        Task<Branch> GetBySolIdAsync(string code, bool includeDeleted=false);
        Task<IList<Branch>> GetAllAsync(bool includeDeleted=false);
        Task<bool> InsertBranchAsync(BranchRequest request);
        Task<bool> UpdateBranchAsync(BranchRequest request);
        Task<bool> DeleteBranchAsync(DeleteRequst request);
        Task<bool> ExistsByIdAsync(long id);
        Task<bool> ExistsAsync(BranchRequest request);

    }

}