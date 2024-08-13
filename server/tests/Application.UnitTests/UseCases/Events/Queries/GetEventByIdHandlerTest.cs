using Application.Common.Errors;
using Application.UseCases.Events.Queries.GetEventById;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Events.Queries;

[TestSubject(typeof(GetEventByIdHandler))]
public class GetEventByIdHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetEventByIdMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public GetEventByIdHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_EventNotExist_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = new GetEventByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null!);

        var handler = new GetEventByIdHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_EventExist_ReturnsOkWithEvent()
    {
        // Arrange
        var command = new GetEventByIdQuery(Guid.NewGuid(), Guid.NewGuid());
        var dbEvent = new Event { Id = command.EventId };
        var userListQueryable = new List<User>().BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);
        
        _eventUserRepositoryMock.Setup(x =>
                x.GetAllUsersFromEvent(It.IsAny<Guid>()))
            .Returns(userListQueryable);

        var handler = new GetEventByIdHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PlacesOccupied.Should().Be(0);
        result.Value.IsRegistered.Should().BeFalse();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}