using Api.Models;
using AutoMapper;
using Shared.Models;

namespace Api;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile(){
        CreateMap<Tag, TagResponseDTO>();
    }
}
