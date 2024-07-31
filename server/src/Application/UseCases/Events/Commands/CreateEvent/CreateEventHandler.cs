using Application.Common.Errors.Base;
using Application.Common.Interfaces.Messaging;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentResults;

namespace Application.UseCases.Events.Commands.CreateEvent;

public class CreateEventHandler(IMapper mapper, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateEventCommand, EventResponse>
{
    public async Task<Result<EventResponse>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var createEvent = mapper.Map<Event>(request);

        await unitOfWork.EventRepository.CreateAsync(createEvent, cancellationToken);

        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (result.IsFailed)
        {
            return new InternalServerError("Event.Create", "Something went wrong when saving the data");
        }

        return mapper.Map<EventResponse>(createEvent);
    }
}