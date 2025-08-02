using AutoMapper;
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
        
        }

    }
}
