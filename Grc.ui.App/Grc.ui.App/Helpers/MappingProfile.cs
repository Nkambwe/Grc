using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;
using Microsoft.AspNetCore.Components;

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

        }
    }
}
