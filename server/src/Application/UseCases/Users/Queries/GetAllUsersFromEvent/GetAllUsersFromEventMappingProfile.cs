using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Users.Queries.GetAllUsersFromEvent;

public class GetAllUsersFromEventMappingProfile : Profile
{
    public GetAllUsersFromEventMappingProfile()
    {
        var eventId = Guid.Empty;
        CreateProjection<User, UserResponse>()
            .ForMember(ur => ur.RegistrationDate, opt => opt.MapFrom(src => src.EventUsers
                .First(eu => eu.EventId == eventId).RegistrationDate));
    }
}