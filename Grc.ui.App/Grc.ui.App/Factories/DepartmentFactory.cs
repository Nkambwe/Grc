using AutoMapper;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Http.Requests;
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
        private readonly IDepartmentService _deptservice;
        public DepartmentFactory(
            IConfiguration configuration,
            IMapper mapper,
            IPinnedService pinnedService, 
            IQuickActionService quickActionService,
            ISystemAccessService accessService,
            IDepartmentService deptservice,
            SessionManager session){
            _sessionManager = session;
            _configuration = configuration; 
            _pinnedService = pinnedService;
            _quickActionService = quickActionService;
            _mapper = mapper;
            _accessService = accessService;
            _deptservice = deptservice;
        }

        public async Task<DepartmentModel> PrepareDepartmentModelAsync(UserModel currentUser) {
            var model = new DepartmentModel(){  
            };
            return await Task.FromResult(model);
        }

        public async Task<DepartmentListModel> PrepareDepartmentListModelAsync(UserModel currentUser) {

            var branchdata = await _deptservice.GetDepartmentsAsync(new GrcRequest() {
                UserId = currentUser.UserId, 
                IPAddress = currentUser.IPAddress,
                Action = "Retrieve departments",
                DecryptFields = Array.Empty<string>(),
                EncryptFields = Array.Empty<string>()
            });

            DepartmentListModel model; 
            if (!branchdata.HasError) {
                model = new DepartmentListModel() {
                    Departments = _mapper.Map<List<DepartmentModel>>(branchdata.Data)
                };
            } else {
                var data = branchdata.Data;
                model = new DepartmentListModel();
                if (data != null && data.Count > 0) {;
                    model.Departments = branchdata.Data != null ?
                        data.Select(b => new DepartmentModel() {
                            Id = b.Id,
                            BranchId = b.BranchId,
                            Branch = b.Branch,
                            DepartmentName = b.DepartmentName,
                            DepartmentAlias = b.DepartmentAlias,
                            DepartmentCode = b.DepartmentCode,
                            IsDeleted = b.IsDeleted,
                            CreatedOn = DateTime.Now.AddDays(-10)   
                        }).ToList() :
                        new List<DepartmentModel>();
                } else {
                    model.Departments = new List<DepartmentModel>();
                }
            }

            return await Task.FromResult(model);
        }
    }
}
