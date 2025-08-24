using AutoMapper;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Reflection;
using System.Text.Json;
using System.Linq;

namespace Grc.ui.App.Services {

    public class WorkspaceService : GrcBaseService, IWorkspaceService {
        private readonly IBranchService _branchService;
        public WorkspaceService(IApplicationLoggerFactory loggerFactory, 
                                IHttpClientFactory httpClientFactory,
                                IEnvironmentProvider environment, 
                                IEndpointTypeProvider endpointType,
                                IHttpHandler handler,
                                IMapper mapper,
                                IBranchService branchService)
            : base(loggerFactory, handler, environment,endpointType, mapper) {
            Logger.Channel = $"WORKSPACE-{DateTime.Now:yyyyMMddHHmmss}";
            _branchService = branchService;
        }

        public async Task<WorkspaceModel> BuildWorkspaceAsync(long userId, string ipAddress) {
            //..get work model
            var grcResponse = await _branchService.GetWorkspaceAsync(userId, userId, ipAddress);
            Logger.LogActivity($"WORKSPACE RESPONSE: {JsonSerializer.Serialize(grcResponse)}");
            if (grcResponse.HasError) {
                return new();
            }

            //..success response
            var workspace = new WorkspaceModel {
                IsLiveEnvironment = Environment.IsLive,
                CurrentUser = Mapper.Map<CurrentUserModel>(grcResponse.Data.CurrentUser),
                Permissions = grcResponse.Data.Permissions,
                Role = grcResponse.Data.Role,
                RoleId = grcResponse.Data.RoleId,
                AssignedBranch = Mapper.Map<BranchModel>(grcResponse.Data.AssignedBranch)
            };

            if(grcResponse.Data.UserViews != null && grcResponse.Data.UserViews.Count > 0){ 
                workspace.UserViews = (from view in grcResponse.Data.UserViews select Mapper.Map<UserViewModel>(view)).ToList();
            }

            return workspace;
        }

        public Task CleanupWorkspaceAsync(string userId) {
           return Task.CompletedTask;
        }

        public Task SaveWorkspaceChangesAsync(WorkspaceModel workspace) {
            //TODO --save preferences
            //TODO --save favourites
             return Task.CompletedTask;
        }

    }
}
