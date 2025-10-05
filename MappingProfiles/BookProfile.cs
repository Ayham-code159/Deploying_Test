using AutoMapper;
using Deploying_Test.Models.Dtos.BookDtos;
using Deploying_Test.Models.Entities;

namespace Deploying_Test.MappingProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book,BookDto>();

            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore());


            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore());  
        }




    }
}
