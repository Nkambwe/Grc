using AutoMapper;

namespace Grc.Middleware.Api.Helpers {
    public class MappingProfile : Profile {

        public MappingProfile() { 
            //CreateMap<Branch, BranchResponse>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(o => o.Id))
            //    .ForMember(dest => dest.Code, opt => opt.MapFrom(o => (o.BranchCode??string.Empty).Trim()))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(o => (o.BranchName ?? string.Empty).Trim()))
            //    .ForMember(dest => dest.Active, opt => opt.MapFrom(o => o.IsActive))
            //    .ForMember(dest => dest.Deleted, opt => opt.MapFrom(o => o.IsDeleted))
            //    .ForMember(dest => dest.AddedBy, opt => opt.MapFrom(o => (o.CreatedBy ?? string.Empty).Trim()))
            //    .ForMember(dest => dest.AddedOn, opt => opt.MapFrom(o => o.CreatedOn))
            //    .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(o => (o.LastModifiedBy ?? string.Empty).Trim()))
            //    .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(o => o.LastModifiedOn));
        }

    }
}
