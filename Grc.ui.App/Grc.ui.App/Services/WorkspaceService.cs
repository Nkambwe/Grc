using AutoMapper;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services {

    public class WorkspaceService : GrcBaseService, IWorkspaceService {
        public WorkspaceService(IApplicationLoggerFactory loggerFactory, 
                                IHttpClientFactory httpClientFactory,
                                IEnvironmentProvider environment, 
                                IEndpointTypeProvider endpointType,
                                IHttpHandler handler,
                                IMapper mapper)
            : base(loggerFactory, handler, environment,endpointType, mapper) {
            Logger.Channel = $"WORKSPACE-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public Task<WorkspaceModel> BuildWorkspaceAsync(string userId) {
           return Task.FromResult(new WorkspaceModel());
        }

        public Task CleanupWorkspaceAsync(string userId) {
           return Task.CompletedTask;
        }

        public Task SaveWorkspaceChangesAsync(WorkspaceModel workspace) {
             return Task.CompletedTask;
        }

    }
}
