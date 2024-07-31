using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Commands.CreateEvent;

public class CreateEventMappingProfile : Profile
{
    public CreateEventMappingProfile()
    {
        CreateMap<CreateEventCommand, Event>();

        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.MapFrom(_ => 0));
    }
}