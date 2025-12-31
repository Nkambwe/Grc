using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {
    public class ControlFactory : IControlFactory {
        private readonly IComplianceControlService _controlService;
        public ControlFactory(IComplianceControlService controlService) {
            _controlService = controlService;
        }
        public async Task<ControlViewModel> PrepareControlViewModelAsync(UserModel currentUser) {
            ControlViewModel controlModel = new() {
                Controls = new(),
                Responsibilities = new()
            };
            GrcRequest request = new() {
                UserId = currentUser.UserId,
                IPAddress = currentUser.IPAddress,
                Action = Activity.COMPLIANCE_CATEGORY_CONTROL.GetDescription(),
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };
            //..get lists of process support items  
            var response = await _controlService.GetControlSupportItemsAsync(request);
            if (response.HasError) {
                return controlModel;
            }
            var supportItems = response.Data;
            if (supportItems == null) {
                return controlModel;
            }
            //..get control items
            if (supportItems.Controls != null && supportItems.Controls.Any()) {
                controlModel.Controls.AddRange(
                    from control in supportItems.Controls
                    select new ControlListItemViewModel {
                        Id = control.Id,
                        ControlName = control.ControlName
                    }
                );
            }

            //..get responsibilites
            if (supportItems.Responsibilities != null && supportItems.Responsibilities.Any()) {
                controlModel.Responsibilities.AddRange(
                    from respo in supportItems.Responsibilities
                    select new ResponsibilityViewModel {
                        Id = respo.Id,
                        ResponsibleRole = respo.ResponsibilityRole
                    }
                );
            }
            return controlModel;
        }
    }
}
