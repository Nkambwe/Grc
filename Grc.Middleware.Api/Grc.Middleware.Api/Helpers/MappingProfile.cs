using AutoMapper;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Requests;

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
        }

    }
}
