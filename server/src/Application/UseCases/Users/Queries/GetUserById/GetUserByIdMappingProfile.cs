using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdMappingProfile : Profile
{
    public GetUserByIdMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}