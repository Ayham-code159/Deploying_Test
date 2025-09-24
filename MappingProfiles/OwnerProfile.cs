using AutoMapper;
using Deploying_Test.Models.Dtos.OwnerDtos;
using Deploying_Test.Models.Entities;

namespace Deploying_Test.MappingProfiles
{
    public class OwnerProfile : Profile
    {
        public OwnerProfile()
        {
            CreateMap<RegisterDto, Owner>()
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.SecurityStamp, opt => opt.Ignore())
                .ForMember(d => d.ConcurrencyStamp, opt => opt.Ignore());

            CreateMap<Owner, OutPutDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName));

        }

    }
}
