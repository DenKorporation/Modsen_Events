using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Events.Commands.UpdateEvent;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Events.Commands;

[TestSubject(typeof(UpdateEventHandler))]
public class UpdateEventHandlerTest
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public UpdateEventHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_EventNotExist_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = new UpdateEventCommand(Guid.NewGuid(), "update", "update", DateTime.Now, "update", "update", 1);

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null!);

        var handler = new UpdateEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EventCapacityConflict_ReturnsEventCapacityConflictError()
    {
        // Arrange
        var command = new UpdateEventCommand(Guid.NewGuid(), "update", "update", DateTime.Now, "update", "update", 1);
        var dbEvent = new Event { Id = command.Id };
        var userEventList = new List<User>
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
        };
        var dataQueryable = userEventList.BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(x =>
                x.GetAllUsersFromEvent(dbEvent.Id))
            .Returns(dataQueryable);

        var handler = new UpdateEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventCapacityConflictError>();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
        
    }
    
    [Fact]
    public async Task Handle_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new UpdateEventCommand(Guid.NewGuid(), "update", "update", DateTime.Now, "update", "update", 1);
        var dbEvent = new Event { Id = command.Id };
        var userEventList = new List<User>
        {
            new() { Id = Guid.NewGuid() },
        };
        var dataQueryable = userEventList.BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(x =>
                x.GetAllUsersFromEvent(dbEvent.Id))
            .Returns(dataQueryable);
        
        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));

        var handler = new UpdateEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(
            x => x.Map(
                It.IsAny<UpdateEventCommand>(),
                It.IsAny<Event>()));
        _eventRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<Event>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SaveChangesSuccess_ReturnsOkWithUpdatedValue()
    {
        // Arrange
        var command = new UpdateEventCommand(Guid.NewGuid(), "update", "update", DateTime.Now, "update", "update", 1);
        var dbEvent = new Event { Id = command.Id };
        var response = new EventResponse { Id = command.Id };
        var userEventList = new List<User>
        {
            new() { Id = Guid.NewGuid() },
        };
        var dataQueryable = userEventList.BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(
                x =>
                x.GetAllUsersFromEvent(dbEvent.Id))
            .Returns(dataQueryable);
        
        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        _mapperMock.Setup(
                x => x.Map<EventResponse>(
                    It.IsAny<Event>()))
            .Returns(response);

        var handler = new UpdateEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(
            x => x.Map(
                It.IsAny<UpdateEventCommand>(),
                It.IsAny<Event>()));
        _eventRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<Event>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}