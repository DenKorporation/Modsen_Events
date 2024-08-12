using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Users.Queries.GetAllUsers;

public class GetAllUsersMappingProfile : Profile
{
    public GetAllUsersMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Birthdate, opt => opt.MapFrom(src => src.Birthday))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles.First()));
    }
}