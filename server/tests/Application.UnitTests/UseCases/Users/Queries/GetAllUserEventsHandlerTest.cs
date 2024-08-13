using Application.Common.Errors;
using Application.UseCases.Users.Queries.GetAllUsersFromEvent;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Users.Queries;

[TestSubject(typeof(GetAllUsersFromEventHandler))]
public class GetAllUsersFromEventHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetAllUsersFromEventMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public GetAllUsersFromEventHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_EventNotExist_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = new GetAllUsersFromEventQuery(Guid.NewGuid(), 1, 5);

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null!);

        var handler = new GetAllUsersFromEventHandler(
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
    public async Task Handle_EventHasUsers_ReturnsOkWithPagedList()
    {
        // Arrange
        var command = new GetAllUsersFromEventQuery(Guid.NewGuid(), 1, 5);
        var dbEvent = new Event { Id = command.EventId };
        var userListQueryable = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Events = [
                    dbEvent
                ],
                EventUsers = [
                    new() { EventId = command.EventId, RegistrationDate = new DateOnly()}
                ]
            },
        }.BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(x =>
                x.GetAllUsersFromEvent(
                    It.Is<Guid>(g => g == command.EventId)))
            .Returns(userListQueryable);

        var handler = new GetAllUsersFromEventHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
    }
}