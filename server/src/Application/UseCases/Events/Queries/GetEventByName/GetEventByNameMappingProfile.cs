using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Queries.GetEventByName;

public class GetEventByNameMappingProfile : Profile
{
    public GetEventByNameMappingProfile()
    {
        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.Ignore());
    }
}