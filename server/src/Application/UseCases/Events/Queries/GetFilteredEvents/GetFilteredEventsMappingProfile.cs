using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Queries.GetFilteredEvents;

public class GetFilteredEventsMappingProfile : Profile
{
    public GetFilteredEventsMappingProfile()
    {
        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.MapFrom(src => (uint)src.Users.Count));
    }
}