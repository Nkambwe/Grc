using AutoMapper;
using Grc.ui.App.Http.Requests;
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
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.SystemAdmin.Password));
    }
    }
}
