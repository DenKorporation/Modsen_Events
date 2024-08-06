using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Users.Commands.CreateUser;

public class CreateUserMappingProfile : Profile
{
    public CreateUserMappingProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Birthday,
                opt => opt.MapFrom(src => DateOnly.ParseExact(src.Birthday, "yyyy-MM-dd")))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<User, UserResponse>();
    }
}