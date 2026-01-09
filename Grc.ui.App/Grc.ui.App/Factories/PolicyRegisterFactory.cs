using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class PolicyRegisterFactory : IPolicyRegisterFactory {

        private readonly IPolicyService _policyService;

        public PolicyRegisterFactory(IPolicyService policyService) {
            _policyService = policyService;
        }

        public async Task<PolicyRegisterViewModel> PreparePolicyRegisterViewModelAsync(UserModel currentUser) {
            PolicyRegisterViewModel policyModel= new() {
                Authorities = new(),
                Responsibilities = new(),
                Departments = new(),
                Frequencies = new(),
                RegulatoryTypes = new()
            };

            GrcRequest request = new() {
                UserId = currentUser.UserId,
                IPAddress = currentUser.IPAddress,
                Action = Activity.RETRIEVEPOLICYSUPPORT.GetDescription(),
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };

            //..get lists of process support items  
            var response = await _policyService.GetPolicySupportItemsAsync(request);
            if (response.HasError) {
                return policyModel;
            }

            var supportItems = response.Data;
            if (supportItems == null) {
                return policyModel;
            }

            //..get departments
            if (supportItems.Departments != null && supportItems.Departments.Any()) {
                policyModel.Departments.AddRange(
                    from department in supportItems.Departments
                    select new DepartmentViewModel {
                        Id = department.Id,
                        DepartmentName = department.DepartmentName
                    }
                );
            }

            //..get responsibilities
            if (supportItems.Responsibilities != null && supportItems.Responsibilities.Any()) {
                policyModel.Responsibilities.AddRange(
                    from owner in supportItems.Responsibilities
                    select new ResponsibilityViewModel {
                        Id = owner.Id,
                        DepartmentName = owner.DepartmentName,
                        ResponsibleRole = owner.ResponsibilityRole
                    }
                );
            }

            //..get frequency
            if (supportItems.Frequencies != null && supportItems.Frequencies.Any()) {
                policyModel.Frequencies.AddRange(
                    from frequency in supportItems.Frequencies
                    select new FrequencyViewModel {
                        Id = frequency.Id,
                        FrequencyName = frequency.FrequencyName
                    }
                );
            }

            //..get authorities
            if (supportItems.Authorities != null && supportItems.Authorities.Any()) {
                policyModel.Authorities.AddRange(
                    from authority in supportItems.Authorities
                    select new AuthorityViewModel {
                        Id = authority.Id,
                        AuthorityName = authority.AuthorityName
                    }
                );
            }

            //..get regulatory types
            if (supportItems.RegulatoryTypes != null && supportItems.RegulatoryTypes.Any()) {
                policyModel.RegulatoryTypes.AddRange(
                    from type in supportItems.RegulatoryTypes
                    select new RegulatoryTypeViewModel {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }

            //..get enforcement laws
            if (supportItems.ReturnTypes != null && supportItems.ReturnTypes.Any()) {
                policyModel.ReturnTypes.AddRange(
                    from type in supportItems.ReturnTypes
                    select new ReturnTypeViewModel {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }

            //..get enforcement laws
            if (supportItems.EnforcementLaws != null && supportItems.EnforcementLaws.Any()) {
                policyModel.EnforcementLaws.AddRange(
                    from law in supportItems.EnforcementLaws
                    select new StatuteMinViewModel {
                        Id = law.Id,
                        Section = law.Section,
                        Requirement = law.Requirement
                    }
                );
            }

            return policyModel;
        }
    }
}
