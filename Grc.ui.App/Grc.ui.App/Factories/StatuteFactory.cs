using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;
using Grc.ui.App.Services;

namespace Grc.ui.App.Factories {

    public class StatuteFactory : IStatuteFactory {

        private readonly IRegulatoryStatuteService _statuteService;

        public StatuteFactory(IRegulatoryStatuteService statuteService) {
            _statuteService = statuteService;
        }

        public async Task<StatutoryViewModel> PrepareStatuteViewModelAsync(UserModel currentUser) {
            StatutoryViewModel statuteModel = new() {
                Authorities = new(),
                Responsibilities = new(),
                Departments = new(),
                Frequencies = new(),
                StatuteTypes = new()
            };

            GrcRequest request = new() {
                UserId = currentUser.UserId,
                IPAddress = currentUser.IPAddress,
                Action = Activity.LAW_RETRIEVE_SUPPORT.GetDescription(),
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };

            //..get lists of process support items  
            var response = await _statuteService.GetStatuteSupportItemsAsync(request);
            if (response.HasError) {
                return statuteModel;
            }

            var supportItems = response.Data;
            if (supportItems == null) {
                return statuteModel;
            }

            //..get departments
            if (supportItems.Departments != null && supportItems.Departments.Any()) {
                statuteModel.Departments.AddRange(
                    from department in supportItems.Departments
                    select new DepartmentViewModel {
                        Id = department.Id,
                        DepartmentName = department.DepartmentName
                    }
                );
            }

            //..get responsibilities
            if (supportItems.Responsibilities != null && supportItems.Responsibilities.Any()) {
                statuteModel.Responsibilities.AddRange(
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
                statuteModel.Frequencies.AddRange(
                    from frequency in supportItems.Frequencies
                    select new FrequencyViewModel {
                        Id = frequency.Id,
                        FrequencyName = frequency.FrequencyName
                    }
                );
            }

            //..get authorities
            if (supportItems.Authorities != null && supportItems.Authorities.Any()) {
                statuteModel.Authorities.AddRange(
                    from authority in supportItems.Authorities
                    select new AuthorityViewModel {
                        Id = authority.Id,
                        AuthorityName = authority.AuthorityName
                    }
                );
            }

            //..get statute types
            if (supportItems.StatuteTypes != null && supportItems.StatuteTypes.Any()) {
                statuteModel.StatuteTypes.AddRange(
                    from type in supportItems.StatuteTypes
                    select new StatuteTypeViewModel {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }

            return statuteModel;
        }

        public async Task<AuditSupportViewModel> PrepareAuditSupportViewModelAsync(UserModel currentUser) {
            AuditSupportViewModel supportModel = new() {
                Authorities = new(),
                Types = new()
            };

            GrcRequest request = new() {
                UserId = currentUser.UserId,
                IPAddress = currentUser.IPAddress,
                Action = Activity.LAW_RETRIEVE_SUPPORT.GetDescription(),
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>()
            };

            //..get lists of process support items  
            var response = await _statuteService.GetAuditSupportItemsAsync(request);
            if (response.HasError) {
                return supportModel;
            }

            var supportItems = response.Data;
            if (supportItems == null) {
                return supportModel;
            }

            //..get authorities
            if (supportItems.Authorities != null && supportItems.Authorities.Any()) {
                supportModel.Authorities.AddRange(
                    from authority in supportItems.Authorities
                    select new AuthorityViewModel {
                        Id = authority.Id,
                        AuthorityName = authority.AuthorityName
                    }
                );
            }

            //..get audit types
            if (supportItems.Types != null && supportItems.Types.Any()) {
                supportModel.Types.AddRange(
                    from type in supportItems.Types
                    select new AuditTypeMiniViewModel {
                        Id = type.Id,
                        TypeName = type.TypeName
                    }
                );
            }

            //..get audits
            if (supportItems.Audits != null && supportItems.Audits.Any()) {
                supportModel.Audits.AddRange(
                    from audit in supportItems.Audits
                    select new AuditMiniViewModel {
                        Id = audit.Id,
                        AuditName = audit.AuditName
                    }
                );
            }

            //..get responsibiliyies
            if (supportItems.Responsibilities != null && supportItems.Responsibilities.Any()) {
                supportModel.Responsibilities.AddRange(
                    from resp in supportItems.Responsibilities
                    select new ResponsibilityViewModel {
                        Id = resp.Id,
                        ResponsibleRole = resp.ResponsibilityRole,
                        DepartmentName = resp.DepartmentName,
                    }
                );
            }

            return supportModel;
        }
    }
}
