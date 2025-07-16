using AutoMapper;
using Grc.Middleware.Api.Data.Entities;
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
        }

    }
}
