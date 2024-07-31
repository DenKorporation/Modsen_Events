using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Queries.GetAllUserEvents;

public class GetAllUserEventsMappingProfile : Profile
{
    public GetAllUserEventsMappingProfile()
    {
        var userId = Guid.Empty;
        CreateProjection<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.MapFrom(src => (uint)src.Users.Count))
            .ForMember(ur => ur.RegistrationDate, opt => opt.MapFrom(src => src.EventUsers
                .First(eu => eu.UserId == userId).RegistrationDate));
    }
}