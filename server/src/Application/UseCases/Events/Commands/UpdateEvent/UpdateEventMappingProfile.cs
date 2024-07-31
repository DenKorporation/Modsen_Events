using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Commands.UpdateEvent;

public class UpdateEventMappingProfile : Profile
{
    public UpdateEventMappingProfile()
    {
        CreateMap<UpdateEventCommand, Event>();

        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.Ignore());
    }
}