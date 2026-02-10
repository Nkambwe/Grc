using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;

namespace Grc.Middleware.Api.Data.Repositories {
    public interface IRoleRepository : IRepository<SystemRole>
    {
        Task<RoleMinResponse> GetRoleWithPermissionsAsync(IdRequest request);
    }
}
