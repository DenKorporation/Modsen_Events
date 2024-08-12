using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdMappingProfile : Profile
{
    public GetUserByIdMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Birthdate, opt => opt.MapFrom(src => src.Birthday));
    }
}