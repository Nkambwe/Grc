using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;

namespace Grc.Middleware.Api.Helpers {
    public class MappingProfile : Profile {

        public MappingProfile() {
            CreateMap<RegistrationRequest, Company>()
                .ForMember(co => co.Id, reg => reg.MapFrom(o => 0))
                .ForMember(co => co.CompanyName, reg => reg.MapFrom(o => (o.CompanyName ?? string.Empty).Trim()))
                .ForMember(co => co.ShortName, reg => reg.MapFrom(o => (o.Alias ?? string.Empty).Trim()))
                .ForMember(co => co.RegistrationNumber, reg => reg.MapFrom(o => (o.RegNumber ?? string.Empty).Trim()))
                .ForMember(co => co.SystemLanguage, reg => reg.MapFrom(o => o.Language))
                .ForMember(co => co.IsDeleted, reg => reg.MapFrom(o => false))
                .ForMember(co => co.CreatedBy, reg => reg.MapFrom(o => (o.CreatedBy ?? string.Empty).Trim()))
                .ForMember(co => co.CreatedOn, reg => reg.MapFrom(o => o.CreatedOn))
                .ForMember(co => co.LastModifiedBy, reg => reg.MapFrom(o => (o.ModifiedBy ?? string.Empty).Trim()))
                .ForMember(co => co.CreatedOn, reg => reg.MapFrom(o => o.CreatedOn));
        
            //..map user from registration
            CreateMap<RegistrationRequest, SystemUser>()
                .ForMember(us => us.Id, reg => reg.MapFrom(o => 0))
                .ForMember(us => us.FirstName, reg => reg.MapFrom(o => (o.FirstName ?? string.Empty).Trim()))
                .ForMember(us => us.LastName, reg => reg.MapFrom(o => (o.LastName ?? string.Empty).Trim()))
                .ForMember(us => us.OtherName, reg => reg.MapFrom(o => (o.MiddleName ?? string.Empty).Trim()))
                .ForMember(us => us.EmailAddress, reg => reg.MapFrom(o => (o.Email ?? string.Empty).Trim()))
                .ForMember(us => us.Username, reg => reg.MapFrom(o => (o.UserName ?? string.Empty).Trim()))
                .ForMember(us => us.PasswordHash, reg => reg.MapFrom(o => (o.Password ?? string.Empty).Trim()))
                .ForMember(us => us.PFNumber, reg => reg.MapFrom(o => (o.PFNumber ?? string.Empty).Trim()))
                .ForMember(us => us.BranchSolId, reg => reg.MapFrom(o => (o.SolId ?? "MAIN").Trim()))
                .ForMember(us => us.PhoneNumber, reg => reg.MapFrom(o => o.PhoneNumber))
                .ForMember(us => us.DepartmentUnit, reg => reg.MapFrom(o => o.UnitCode))
                .ForMember(us => us.RoleId, reg => reg.MapFrom(o => o.RoleId))
                .ForMember(us => us.DepartmentId, reg => reg.MapFrom(o => o.DepartmentId))
                .ForMember(us => us.IsActive, reg => reg.MapFrom(o => true))
                .ForMember(us => us.IsVerified, reg => reg.MapFrom(o => true))
                .ForMember(us => us.IsDeleted, reg => reg.MapFrom(o => false))
                .ForMember(us => us.CreatedBy, reg => reg.MapFrom(o => (o.CreatedBy ?? string.Empty).Trim()))
                .ForMember(us => us.CreatedOn, reg => reg.MapFrom(o => o.CreatedOn))
                .ForMember(us => us.LastModifiedBy, reg => reg.MapFrom(o => (o.ModifiedBy ?? string.Empty).Trim()))
                .ForMember(us => us.CreatedOn, reg => reg.MapFrom(o => o.CreatedOn));

            //..map user from registration
            CreateMap<SystemUser, AuthenticationResponse>()
                .ForMember(a => a.UserId, reg => reg.MapFrom(o => o.Id))
                .ForMember(a => a.FirstName, reg => reg.MapFrom(o => (o.FirstName ?? string.Empty).Trim()))
                .ForMember(a => a.LastName, reg => reg.MapFrom(o => (o.LastName ?? string.Empty).Trim()))
                .ForMember(a => a.MiddleName, reg => reg.MapFrom(o => (o.OtherName ?? string.Empty).Trim()))
                .ForMember(a => a.EmailAddress, reg => reg.MapFrom(o => (o.EmailAddress ?? string.Empty).Trim()))
                .ForMember(a => a.Username, reg => reg.MapFrom(o => (o.Username ?? string.Empty).Trim()))
                .ForMember(a => a.Password, reg => reg.MapFrom(o => (o.PasswordHash ?? string.Empty).Trim()))
                .ForMember(a => a.PFNumber, reg => reg.MapFrom(o => (o.PFNumber ?? string.Empty).Trim()))
                .ForMember(a => a.SolId, reg => reg.MapFrom(o => (o.BranchSolId ?? "MAIN").Trim()))
                .ForMember(a => a.PhoneNumber, reg => reg.MapFrom(o => o.PhoneNumber))
                .ForMember(a => a.RoleId, reg => reg.MapFrom(o => o.RoleId))
                .ForMember(a => a.RoleName, reg => reg.MapFrom(o => o.Role.RoleName))
                .ForMember(a => a.DepartmentId, reg => reg.MapFrom(o => o.DepartmentId))
                .ForMember(a => a.DepartmentCode, reg => reg.MapFrom(o => o.Department.DepartmenCode))
                .ForMember(a => a.UnitCode, reg => reg.MapFrom(o => o.DepartmentUnit))
                .ForMember(a => a.IsActive, reg => reg.MapFrom(o => o.IsActive))
                .ForMember(a => a.IsVerified, reg => reg.MapFrom(o => o.IsVerified))
                .ForMember(a => a.IsApproved, reg => reg.MapFrom(o => o.IsApproved))
                .ForMember(a => a.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(a => a.RedirectUrl, reg => reg.MapFrom(o => string.Empty))
                .ForMember(a => a.Favourites, reg => reg.MapFrom(o => new List<string>()))
                .ForMember(a => a.Views, reg => reg.MapFrom(o => new List<string>()))
                .ForMember(a => a.Claims, reg => reg.MapFrom(o => new Dictionary<string, object>()));

            //..map quick actions
            CreateMap<UserQuickAction, QuickActionResponse>()
                .ForMember(a => a.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(a => a.Label, reg => reg.MapFrom(o => (o.Label ?? string.Empty).Trim()))
                .ForMember(a => a.IconClass, reg => reg.MapFrom(o => (o.IconClass ?? string.Empty).Trim()))
                .ForMember(a => a.Controller, reg => reg.MapFrom(o => (o.Controller ?? string.Empty).Trim()))
                .ForMember(a => a.Action, reg => reg.MapFrom(o => (o.Action ?? string.Empty).Trim()))
                .ForMember(a => a.Area, reg => reg.MapFrom(o => (o.Area ?? string.Empty).Trim()))
                .ForMember(a => a.CssClass, reg => reg.MapFrom(o => (o.CssClass ?? string.Empty).Trim()));

            //..map pinned items
            CreateMap<UserPinnedItem, PinnedItemResponse>()
                .ForMember(a => a.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(a => a.Label, reg => reg.MapFrom(o => (o.Label ?? string.Empty).Trim()))
                .ForMember(a => a.IconClass, reg => reg.MapFrom(o => (o.IconClass ?? string.Empty).Trim()))
                .ForMember(a => a.Controller, reg => reg.MapFrom(o => (o.Controller ?? string.Empty).Trim()))
                .ForMember(a => a.Action, reg => reg.MapFrom(o => (o.Action ?? string.Empty).Trim()))
                .ForMember(a => a.Area, reg => reg.MapFrom(o => (o.Area ?? string.Empty).Trim()))
                .ForMember(a => a.CssClass, reg => reg.MapFrom(o => (o.CssClass ?? string.Empty).Trim()));

            
            //..map pinned items
            CreateMap<ErrorRequest, SystemError>()
                .ForMember(e => e.CompanyId, reg => reg.MapFrom(o => o.CompanyId))
                .ForMember(e => e.ErrorMessage, reg => reg.MapFrom(o => (o.Message ?? string.Empty).Trim()))
                .ForMember(e => e.ErrorSource, reg => reg.MapFrom(o => (o.Source ?? string.Empty).Trim()))
                .ForMember(e => e.StackTrace, reg => reg.MapFrom(o => (o.StackTrace ?? string.Empty).Trim()))
                .ForMember(e => e.Severity, reg => reg.MapFrom(o => (o.Severity ?? string.Empty).Trim()))
                .ForMember(e => e.Assigned, reg => reg.MapFrom(o => false))
                .ForMember(e => e.IsDeleted, reg => reg.MapFrom(o => false))
                .ForMember(e => e.FixStatus, reg => reg.MapFrom(o => "OPEN"))
                .ForMember(e => e.ReportedOn, reg => reg.MapFrom(o => DateTime.Now))
                .ForMember(e => e.CreatedOn, reg => reg.MapFrom(o => DateTime.Now))
                .ForMember(e => e.CreatedBy, reg => reg.MapFrom(o => $"{o.UserId}"))
                .ForMember(e => e.IsUserReported, reg => reg.MapFrom(o => false));

             CreateMap<ActivityLog, ActivityLogResponse>()
                .ForMember(r => r.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(r => r.UserId, reg => reg.MapFrom(o => o.UserId))
                .ForMember(r => r.UserFirstName, reg => reg.MapFrom(o => (o.User.FirstName ?? string.Empty).Trim()))
                .ForMember(r => r.UserLastName, reg => reg.MapFrom(o => (o.User.LastName ?? string.Empty).Trim()))
                .ForMember(r => r.UserEmail, reg => reg.MapFrom(o => (o.User.EmailAddress ?? string.Empty).Trim()))
                .ForMember(r => r.EntityId, reg => reg.MapFrom(o => o.EntityId))
                .ForMember(r => r.EntityName, reg => reg.MapFrom(o => (o.EntityName ?? string.Empty).Trim()))
                .ForMember(r => r.TypeId, reg => reg.MapFrom(o => o.TypeId))
                .ForMember(r => r.TypeDescription, reg => reg.MapFrom(o => (o.ActivityType.Description ?? string.Empty).Trim()))
                .ForMember(r => r.IpAddress, reg => reg.MapFrom(o => (o.IpAddress ?? string.Empty).Trim()))
                .ForMember(r => r.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(r => r.CreatedOn, reg => reg.MapFrom(o => o.CreatedOn))
                .ForMember(r => r.Comment, reg => reg.MapFrom(o => (o.Comment ?? string.Empty).Trim()));
            
            CreateMap<Department, DepartmentResponse>()
                .ForMember(r => r.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(r => r.BranchId, reg => reg.MapFrom(o => o.BranchId))
                .ForMember(r => r.Branch, reg => reg.MapFrom(o => (o.Branch.BranchName ?? string.Empty).Trim()))
                .ForMember(r => r.DepartmentCode, reg => reg.MapFrom(o => (o.DepartmenCode ?? string.Empty).Trim()))
                .ForMember(r => r.DepartmentName, reg => reg.MapFrom(o => (o.DepartmentName?? string.Empty).Trim()))
                .ForMember(r => r.DepartmentAlias, reg => reg.MapFrom(o => o.Alias))
                .ForMember(r => r.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(r => r.CreatdOn, reg => reg.MapFrom(o => o.CreatedOn));

            CreateMap<DepartmentUnit, DepartmentUnitResponse>()
                .ForMember(r => r.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(r => r.DepartmentId, reg => reg.MapFrom(o => o.DepartmentId))
                .ForMember(r => r.Department, reg => reg.MapFrom(o => (o.Department.DepartmentName ?? string.Empty).Trim()))
                .ForMember(r => r.UnitName, reg => reg.MapFrom(o => (o.UnitName  ?? string.Empty).Trim()))
                .ForMember(r => r.UnitCode, reg => reg.MapFrom(o => (o.UnitCode?? string.Empty).Trim()))
                .ForMember(r => r.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(r => r.CreatdOn, reg => reg.MapFrom(o => o.CreatedOn));
            
            CreateMap<Branch, BranchResponse>()
                .ForMember(r => r.Id, reg => reg.MapFrom(o => o.Id))
                .ForMember(r => r.CompanyId, reg => reg.MapFrom(o => o.CompanyId))
                .ForMember(r => r.CompanyName, reg => reg.MapFrom(o => (o.Company.CompanyName ?? string.Empty).Trim()))
                .ForMember(r => r.BranchName, reg => reg.MapFrom(o => (o.BranchName ?? string.Empty).Trim()))
                .ForMember(r => r.SolId, reg => reg.MapFrom(o => (o.SolId  ?? string.Empty).Trim()))
                .ForMember(r => r.CreatedBy, reg => reg.MapFrom(o => (o.CreatedBy?? string.Empty).Trim()))
                .ForMember(r => r.CreatedOn, reg => reg.MapFrom(o => o.CreatedOn))
                .ForMember(r => r.IsDeleted, reg => reg.MapFrom(o => o.IsDeleted))
                .ForMember(r => r.LastModifiedBy, reg => reg.MapFrom(o => (o.LastModifiedBy  ?? string.Empty).Trim()))
                .ForMember(r => r.LastModifiedOn, reg => reg.MapFrom(o => o.LastModifiedOn));
        }

    }
}
