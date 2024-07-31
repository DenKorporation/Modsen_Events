using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Queries.GetEventById;

public class GetEventByIdMappingProfile : Profile
{
    public GetEventByIdMappingProfile()
    {
        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.Ignore());
    }
}