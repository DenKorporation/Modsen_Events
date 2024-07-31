using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsMappingProfile : Profile
{
    public GetAllEventsMappingProfile()
    {
        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.PlacesOccupied, opt => opt.MapFrom(src => (uint)src.Users.Count));
    }
}