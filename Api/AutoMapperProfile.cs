using Api.Models;
using AutoMapper;
using Shared.Models;

namespace Api;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile(){
        CreateMap<Tag, TagResponseDTO>();
        CreateMap<TagRequestDTO, Tag>().ForMember(t => t.Name, opt => opt.MapFrom( src => src.Name.ToLower()));
        CreateMap<User, UserResponseDTO>();
        CreateMap<UserRequestDTO, User>().ForMember(user => user.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
