using AutoMapper;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Factories {
    public class DepartmentFactory : IDepartmentFactory {

        private readonly SessionManager _sessionManager;
        private readonly IConfiguration _configuration;
         private readonly IPinnedService _pinnedService;
        private readonly IQuickActionService _quickActionService;
        private readonly IMapper _mapper;
        private readonly ISystemAccessService _accessService;

        public DepartmentFactory(
            IConfiguration configuration,
            IMapper mapper,
            IPinnedService pinnedService, 
            IQuickActionService quickActionService,
            ISystemAccessService accessService,
            SessionManager session){
            _sessionManager = session;
            _configuration = configuration; 
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _accessService = accessService;

        }

        public async Task<DepartmentModel> PrepareDepartmentModelAsync(UserModel currentUser) {
            var model = new DepartmentModel(){  
            };
            return await Task.FromResult(model);
        }

        public async Task<DepartmentListModel> PrepareDepartmentListModelAsync(UserModel currentUser) {
            var model = new DepartmentListModel {
                Departments = new List<DepartmentModel>() {
                    new() {
                        DepartmentCode = "DIT",
                        DepartmentName = "Degitization and Innovation",
                        DepartmentAlias = "Digitization",
                        IsDeleted = false,
                        CreatdOn = DateTime.Now.AddDays(-100)

                    },
                    new() {
                        DepartmentCode = "BIT",
                        DepartmentName = "Business Technology",
                        DepartmentAlias = "BT",
                        IsDeleted = false,
                        CreatdOn = DateTime.Now.AddDays(-80)

                    }
                }
            };
            return await Task.FromResult(model);
        }
    }
}
