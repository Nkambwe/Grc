using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;

namespace Grc.ui.App.Helpers {

    public class MappingProfile: Profile {

        public MappingProfile() { 
             CreateMap<CompanyRegistrationModel, CompanyRegiatrationRequest>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(dest => dest.RegNumber, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.SystemLanguage))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.SystemAdmin.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.SystemAdmin.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.SystemAdmin.MiddleName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SystemAdmin.EmailAddress))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.SystemAdmin.PhoneNumber))
                .ForMember(dest => dest.PFNumber, opt => opt.MapFrom(src => src.SystemAdmin.PFNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.SystemAdmin.UserName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.SystemAdmin.Password))
                .ForMember(dest => dest.SolId, opt => opt.MapFrom(src => "000"))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.UnitCode, opt => opt.MapFrom(src => "00"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsLogged, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => Activity.COMPANYREGISTRATION.GetDescription()))
                .ForMember(dest => dest.EncryptFields, opt => opt.MapFrom(src => new string[]{"FirstName", "LastName", "OtherName", "Email", "PhoneNumber", "PFNumber", "Password" }))
                .ForMember(dest => dest.DecryptFields, opt => opt.MapFrom(src => new string[]{ }))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "1"))
                .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "1"));

            CreateMap<UserViewModel, UserModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
               .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.PFNumber, opt => opt.MapFrom(src => src.PFNumber))
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
               .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
               .ForMember(dest => dest.SolId, opt => opt.MapFrom(src => src.SolId))
               .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
               .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
               .ForMember(dest => dest.UnitCode, opt => opt.MapFrom(src => src.UnitCode))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
               .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified))
               .ForMember(dest => dest.IsLogged, opt => opt.MapFrom(src => src.IsLogged))
               .ForMember(dest => dest.EncryptFields, opt => opt.MapFrom(src => new string[] { "FirstName", "LastName", "OtherName", "Email", "PhoneNumber", "PFNumber", "Password" }))
               .ForMember(dest => dest.DecryptFields, opt => opt.MapFrom(src => new string[] { }))
               .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => string.Empty))
               .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(src => DateTime.Now))
               .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => string.Empty));

            CreateMap<GrcStatuteSectionRequest, StatuteSectionViewModel>()
                .ForMember(r => r.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(r => r.StatutoryId, reg => reg.MapFrom(o => o.StatutoryId))
                .ForMember(r => r.Section, reg => reg.MapFrom(o => (o.Section ?? string.Empty).Trim()))
                .ForMember(r => r.Summery, reg => reg.MapFrom(o => (o.Summery ?? string.Empty).Trim()))
                .ForMember(r => r.Obligation, reg => reg.MapFrom(o => o.Obligation))
                .ForMember(r => r.IsMandatory, reg => reg.MapFrom(o => o.IsMandatory))
                .ForMember(r => r.ExcludeFromCompliance, reg => reg.MapFrom(o => o.ExcludeFromCompliance))
                .ForMember(r => r.Coverage, reg => reg.MapFrom(o => o.Coverage))
                .ForMember(r => r.IsCovered, reg => reg.MapFrom(o => o.IsCovered))
                .ForMember(r => r.FrequencyId, reg => reg.MapFrom(o => o.FrequencyId))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(r => r.ComplianceAssurance, reg => reg.MapFrom(o => o.ComplianceAssurance))
                .ForMember(r => r.Comments, reg => reg.MapFrom(o => (o.Comments ?? string.Empty).Trim()))
                .ForMember(r => r.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted));

            CreateMap<StatuteSectionViewModel, GrcStatuteSectionRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StatutoryId, opt => opt.MapFrom(src => src.StatutoryId))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => src.Section ?? string.Empty))
                .ForMember(dest => dest.Summery, opt => opt.MapFrom(src => src.Summery ?? string.Empty))
                .ForMember(dest => dest.Obligation, opt => opt.MapFrom(src => src.Obligation ?? string.Empty))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.IsMandatory, opt => opt.MapFrom(src => src.IsMandatory))
                .ForMember(dest => dest.ExcludeFromCompliance, opt => opt.MapFrom(src => src.ExcludeFromCompliance))
                .ForMember(dest => dest.Coverage, opt => opt.MapFrom(src => src.Coverage))
                .ForMember(dest => dest.IsCovered, opt => opt.MapFrom(src => src.IsCovered))
                .ForMember(dest => dest.FrequencyId, opt => opt.MapFrom(src => src.FrequencyId))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.ComplianceAssurance, opt => opt.MapFrom(src => src.ComplianceAssurance))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<UsernameValidationModel, UsernameValidationRequest>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.IPAddress, opt => opt.MapFrom(src => src.IPAddress))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => Activity.USERNAMEVALIDATION.GetDescription()))
                .ForMember(dest => dest.EncryptFields, opt => opt.MapFrom(src => src.Encrypt))
                .ForMember(dest => dest.DecryptFields, opt => opt.MapFrom(src => src.Decrypt));
        
            //..map usersignin
            CreateMap<LoginModel, UserSignInRequest>()
                .ForMember(u => u.UserId, reg => reg.MapFrom(o => 0))
                .ForMember(u => u.Username, reg => reg.MapFrom(o => (o.Username ?? string.Empty).Trim()))
                .ForMember(u => u.Password, reg => reg.MapFrom(o => (o.Password ?? string.Empty).Trim()))
                .ForMember(u => u.IsPersistent, reg => reg.MapFrom(o => o.RememberMe))
                .ForMember(u => u.IsValidated, reg => reg.MapFrom(o => o.IsUsernameValidated))
                .ForMember(u => u.Action, opt => opt.MapFrom(src => Activity.AUTHENTICATE.GetDescription()))
                .ForMember(u => u.EncryptFields, opt => opt.MapFrom(src => new string[]{ }))
                .ForMember(u => u.DecryptFields, opt => opt.MapFrom(src => new string[]{"FirstName", "LastName", "OtherName", "Email", "PhoneNumber", "PFNumber"}));
                
            //..map logout model
            CreateMap<LogoutModel,LogoutRequest>()
                .ForMember(u => u.UserId, reg => reg.MapFrom(o => o.UserId))
                .ForMember(u => u.IPAddress, reg => reg.MapFrom(o => o.IPAddress))
                .ForMember(u => u.Action, opt => opt.MapFrom(src => Activity.AUTHENTICATE.GetDescription()))
                .ForMember(u => u.EncryptFields, opt => opt.MapFrom(src => new string[]{ }))
                .ForMember(u => u.DecryptFields, opt => opt.MapFrom(src => new string[]{"FirstName", "LastName", "OtherName", "Email", "PhoneNumber", "PFNumber"}));

            //..map actions
            CreateMap<QuickAction, QuickActionModel>()
                .ForMember(q => q.Label, reg => reg.MapFrom(o => (o.Label ?? string.Empty).Trim()))
                .ForMember(q => q.IconClass, reg => reg.MapFrom(o => (o.IconClass ?? string.Empty).Trim()))
                .ForMember(q => q.Action, reg => reg.MapFrom(o => (o.Action ?? string.Empty).Trim()))
                .ForMember(q => q.Controller, reg => reg.MapFrom(o => o.Controller))
                .ForMember(q => q.Area, reg => reg.MapFrom(o => o.Area))
                .ForMember(q => q.CssClass, reg => reg.MapFrom(o => o.CssClass));
            
            //..map actions
            CreateMap<QuickActionModel, QuickAction>()
                .ForMember(q => q.Label, reg => reg.MapFrom(o => (o.Label ?? string.Empty).Trim()))
                .ForMember(q => q.IconClass, reg => reg.MapFrom(o => (o.IconClass ?? string.Empty).Trim()))
                .ForMember(q => q.Action, reg => reg.MapFrom(o => (o.Action ?? string.Empty).Trim()))
                .ForMember(q => q.Controller, reg => reg.MapFrom(o => o.Controller))
                .ForMember(q => q.Area, reg => reg.MapFrom(o => o.Area))
                .ForMember(q => q.CssClass, reg => reg.MapFrom(o => o.CssClass));
            
            //..map pins
            CreateMap<PinnedItem, PinnedModel>()
                .ForMember(q => q.Label, reg => reg.MapFrom(o => (o.Label ?? string.Empty).Trim()))
                .ForMember(q => q.IconClass, reg => reg.MapFrom(o => (o.IconClass ?? string.Empty).Trim()))
                .ForMember(q => q.Action, reg => reg.MapFrom(o => (o.Action ?? string.Empty).Trim()))
                .ForMember(q => q.Controller, reg => reg.MapFrom(o => o.Controller))
                .ForMember(q => q.Area, reg => reg.MapFrom(o => o.Area))
                .ForMember(q => q.CssClass, reg => reg.MapFrom(o => o.CssClass));
            
            //..map pins
            CreateMap<PinnedModel, PinnedItem>()
                .ForMember(q => q.Label, reg => reg.MapFrom(o => (o.Label ?? string.Empty).Trim()))
                .ForMember(q => q.IconClass, reg => reg.MapFrom(o => (o.IconClass ?? string.Empty).Trim()))
                .ForMember(q => q.Action, reg => reg.MapFrom(o => (o.Action ?? string.Empty).Trim()))
                .ForMember(q => q.Controller, reg => reg.MapFrom(o => o.Controller))
                .ForMember(q => q.Area, reg => reg.MapFrom(o => o.Area))
                .ForMember(q => q.CssClass, reg => reg.MapFrom(o => o.CssClass));

            CreateMap<CurrentUserResponse, CurrentUserModel>()
                .ForMember(m => m.UserId, reg => reg.MapFrom(o => o.UserId))
                .ForMember(m => m.PersonnelFileNumber, reg => reg.MapFrom(o => (o.PersonnelFileNumber ?? string.Empty).Trim()))
                .ForMember(m => m.Username, reg => reg.MapFrom(o => (o.Username ?? string.Empty).Trim()))
                .ForMember(m => m.Email, reg => reg.MapFrom(o => o.Email))
                .ForMember(m => m.FirstName, reg => reg.MapFrom(o => o.FirstName))
                .ForMember(m => m.LastName, reg => reg.MapFrom(o => o.LastName));

            CreateMap<AssignedBranchResponse, BranchModel>()
                .ForMember(b => b.BranchId, reg => reg.MapFrom(o => o.BranchId))
                .ForMember(b => b.OrganizationId, reg => reg.MapFrom(o => o.OrganizationId))
                .ForMember(b => b.SolId, reg => reg.MapFrom(o => (o.SolId ?? string.Empty).Trim()))
                .ForMember(b => b.OrganizationName, reg => reg.MapFrom(o => o.OrganizationName))
                .ForMember(b => b.OrgAlias, reg => reg.MapFrom(o => o.OrgAlias))
                .ForMember(b => b.BranchName, reg => reg.MapFrom(o => o.BranchName));
            
            CreateMap<PreferenceResponse, UserPreferenceModel>()
                .ForMember(p => p.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(p => p.Theme, reg => reg.MapFrom(o => (o.Theme ?? string.Empty).Trim()))
                .ForMember(p => p.Language, reg => reg.MapFrom(o => (o.Language ?? string.Empty).Trim()));

            CreateMap<UserViewResponse, UserViewModel>()
                .ForMember(v => v.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(v => v.FullName, reg => reg.MapFrom(o => (o.Name ?? string.Empty).Trim()))
                .ForMember(v => v.View, reg => reg.MapFrom(o => o.View));

            CreateMap<GrcErrorModel, ErrorRequest>()
                .ForMember(e => e.CompanyId, reg => reg.MapFrom(o => o.CompanyId))
                .ForMember(e => e.UserId, reg => reg.MapFrom(o => o.UserId))
                .ForMember(e => e.Message, reg => reg.MapFrom(o => (o.Message ?? string.Empty).Trim()))
                .ForMember(e => e.Source, reg => reg.MapFrom(o => (o.Source ?? string.Empty).Trim()))
                .ForMember(e => e.Severity, reg => reg.MapFrom(o => (o.Severity ?? "ERROR").Trim()))
                .ForMember(e => e.StackTrace, reg => reg.MapFrom(o => o.StackTrace));

            CreateMap<GrcPermissionSetViewModel, GrcPermissionSetRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(e => e.SetName, reg => reg.MapFrom(o => (o.SetName ?? string.Empty).Trim()))
                .ForMember(e => e.Description, reg => reg.MapFrom(o => (o.Description ?? string.Empty).Trim()))
                .ForMember(e => e.Roles, reg => reg.MapFrom(o => o.Roles ?? new()))
                .ForMember(e => e.Permissions, reg => reg.MapFrom(o => o.Permissions ?? new()))
                .ForMember(e => e.RoleGroups, reg => reg.MapFrom(o => o.RoleGroups ?? new()));

            CreateMap<GrcPermissionSetViewModel, GrcPermissionSetRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(e => e.SetName, reg => reg.MapFrom(o => (o.SetName ?? string.Empty).Trim()))
                .ForMember(e => e.Description, reg => reg.MapFrom(o => (o.Description ?? string.Empty).Trim()))
                .ForMember(e => e.Roles, reg => reg.MapFrom(o => o.Roles ?? new()))
                .ForMember(e => e.Permissions, reg => reg.MapFrom(o => o.Permissions ?? new()))
                .ForMember(e => e.RoleGroups, reg => reg.MapFrom(o => o.RoleGroups ?? new()));

            CreateMap<RoleGroupViewModel, GrcRoleGroupRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(e => e.GroupName, reg => reg.MapFrom(o => (o.GroupName ?? string.Empty).Trim()))
                .ForMember(e => e.GroupDescription, reg => reg.MapFrom(o => (o.GroupDescription ?? string.Empty).Trim()))
                .ForMember(e => e.DepartmentName, reg => reg.MapFrom(o => (o.DepartmentName ?? string.Empty).Trim()))
                .ForMember(e => e.GroupScope, reg => reg.MapFrom(o => 2))
                .ForMember(q => q.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(q => q.IsVerified, reg => reg.MapFrom(o => o.IsVerified))
                .ForMember(q => q.IsApproved, reg => reg.MapFrom(o => o.IsApproved))
                .ForMember(e => e.PermissionSets, reg => reg.MapFrom(o => o.PermissionSets ?? new()))
                .ForMember(e => e.Roles, reg => reg.MapFrom(o => o.Roles ?? new()));

            CreateMap<RoleViewModel, GrcRoleRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(e => e.RoleName, reg => reg.MapFrom(o => (o.RoleName ?? string.Empty).Trim()))
                .ForMember(e => e.RoleDescription, reg => reg.MapFrom(o => (o.RoleDescription ?? string.Empty).Trim()))
                .ForMember(q => q.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(q => q.IsVerified, reg => reg.MapFrom(o => o.IsVerified))
                .ForMember(q => q.IsApproved, reg => reg.MapFrom(o => o.IsApproved))
                .ForMember(e => e.Permissions, reg => reg.MapFrom(o => o.Permissions ?? new()))
                .ForMember(e => e.Users, reg => reg.MapFrom(o => o.Users ?? new()));

            CreateMap<ProcessViewModel, GrcProcessRegisterRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.Id == 0))
                .ForMember(dest => dest.ProcessName, opt => opt.MapFrom(src => src.ProcessName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CurrentVersion, opt => opt.MapFrom(src => src.CurrentVersion))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.ProcessStatus, opt => opt.MapFrom(src => src.ProcessStatus))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.ApprovalStatus))
                .ForMember(dest => dest.ApprovalComment, opt => opt.MapFrom(src => src.ApprovalComment))
                .ForMember(dest => dest.OnholdReason, opt => opt.MapFrom(src => src.OnholdReason))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.IsLockProcess, opt => opt.MapFrom(src => src.IsLocked))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.NeedsBranchReview, opt => opt.MapFrom(src => src.NeedsBranchReview))
                .ForMember(dest => dest.NeedsCreditReview, opt => opt.MapFrom(src => src.NeedsCreditReview))
                .ForMember(dest => dest.NeedsTreasuryReview, opt => opt.MapFrom(src => src.NeedsTreasuryReview))
                .ForMember(dest => dest.NeedsFintechReview, opt => opt.MapFrom(src => src.NeedsFintechReview))
                .ForMember(dest => dest.ResponsibilityId, opt => opt.MapFrom(src => src.ResponsibilityId))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "SYSTEM"))
                .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "SYSTEM"));

            CreateMap<PolicyViewModel, GrcPolicyDocumentRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentName, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.DocumentTypeId, opt => opt.MapFrom(src => src.DocumentTypeId))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DocumentStatus))
                .ForMember(dest => dest.IsAligned, opt => opt.MapFrom(src => src.IsAligned))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.ApprovalDate, opt => opt.MapFrom(src => src.ApprovalDate))
                .ForMember(dest => dest.FrequencyId, opt => opt.MapFrom(src => src.FrequencyId))
                .ForMember(dest => dest.ResponsibilityId, opt => opt.MapFrom(src => src.ResponsibilityId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.LastRevisionDate, opt => opt.MapFrom(src => src.LastRevisionDate))
                .ForMember(dest => dest.NextRevisionDate, opt => opt.MapFrom(src => src.NextRevisionDate));

            CreateMap<StatuteViewModel, GrcStatutoryLawRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.StatuteCode))
                .ForMember(dest => dest.RegulatoryName, opt => opt.MapFrom(src => src.StatuteName))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.StatuteTypeId))
                .ForMember(dest => dest.AuthorityId, opt => opt.MapFrom(src => src.AuthorityId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));

            CreateMap<ProcessGroupViewModel, GrcProcessGroupRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(e => e.GroupName, reg => reg.MapFrom(o => (o.GroupName ?? string.Empty).Trim()))
                .ForMember(e => e.GroupDescription, reg => reg.MapFrom(o => (o.GroupDescription ?? string.Empty).Trim()))
                .ForMember(q => q.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(e => e.Processes, reg => reg.MapFrom(o => o.Processes ?? new()));

            CreateMap<GrcComplianceMapViewMode, GrcComplianceMapRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(q => q.Include, reg => reg.MapFrom(o => o.Include))
                .ForMember(e => e.MapDescription, reg => reg.MapFrom(o => (o.MapDescription ?? string.Empty).Trim()))
                .ForMember(e => e.ControlMaps, reg => reg.MapFrom(o => o.ControlMaps));

            CreateMap<GrcControlMapViewModel, GrcControlMapRequest>()
                .ForMember(e => e.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(q => q.Include, reg => reg.MapFrom(o => o.Include))
                .ForMember(e => e.ControlDescription, reg => reg.MapFrom(o => (o.ControlDescription ?? string.Empty).Trim()))
                .ForMember(e => e.ParentId, reg => reg.MapFrom(o => o.ParentId));

            CreateMap<ObligationMapViewModel, GrcObligationMapRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsMandatory, opt => opt.MapFrom(src => src.IsMandatory))
                .ForMember(dest => dest.Coverage, opt => opt.MapFrom(src => src.Coverage))
                .ForMember(dest => dest.IsCovered, opt => opt.MapFrom(src => src.IsCovered))
                .ForMember(dest => dest.Exclude, opt => opt.MapFrom(src => src.Exclude))
                .ForMember(dest => dest.Assurance, opt => opt.MapFrom(src => src.Assurance))
                .ForMember(dest => dest.Rationale, opt => opt.MapFrom(src => src.Rationale))
                .ForMember(dest => dest.ComplianceMaps, opt => opt.MapFrom(src => src.ComplianceMaps));
        }
    }
}
