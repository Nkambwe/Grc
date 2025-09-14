using AutoMapper;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Text.Json;
using Grc.ui.App.Factories;

namespace Grc.ui.App.Services {

    public class WorkspaceService : GrcBaseService, IWorkspaceService {
        private readonly IBranchService _branchService;
        public WorkspaceService(IApplicationLoggerFactory loggerFactory, 
                                IHttpClientFactory httpClientFactory,
                                IEnvironmentProvider environment, 
                                IEndpointTypeProvider endpointType,
                                IHttpHandler httpHandler,
                                IMapper mapper,
                                IBranchService branchService,
                                IWebHelper webHelper,
                                SessionManager sessionManager,
                                IGrcErrorFactory errorFactory,
                                IErrorService errorService) 
        : base(loggerFactory, httpHandler, environment, endpointType, mapper,webHelper,sessionManager,errorFactory,errorService) {
            Logger.Channel = $"WORKSPACE-{DateTime.Now:yyyyMMddHHmmss}";
            _branchService = branchService;
        }

        public async Task<WorkspaceModel> BuildWorkspaceAsync(long userId, string ipAddress) {

            try{
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
            }catch(Exception ex){
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"WORKSPACE-SERVICE" , ex.StackTrace);
                return null;
            }
        }

        public async Task CleanupWorkspaceAsync(string userId) {
            try {
                return;
            }catch(Exception ex){
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"WORKSPACE-SERVICE" , ex.StackTrace);
                return;
            }
           
        }

        public async Task SaveWorkspaceChangesAsync(WorkspaceModel workspace) {
            try {
                return;
            }catch(Exception ex){
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                await ProcessErrorAsync(ex.Message,"WORKSPACE-SERVICE" , ex.StackTrace);
                return;
            }
        }

    }
}
