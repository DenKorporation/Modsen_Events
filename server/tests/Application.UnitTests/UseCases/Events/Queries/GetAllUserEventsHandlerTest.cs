using Application.Common.Errors;
using Application.UseCases.Events.Queries.GetAllUserEvents;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Events.Queries;

[TestSubject(typeof(GetAllUserEventsHandler))]
public class GetAllUserEventsHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetAllUserEventsMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public GetAllUserEventsHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new GetAllUserEventsQuery(Guid.NewGuid(), 1, 5);

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new GetAllUserEventsHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.UserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UserHasEvents_ReturnsOkWithPagedList()
    {
        // Arrange
        var command = new GetAllUserEventsQuery(Guid.NewGuid(), 1, 5);
        var user = new User { Id = command.UserId };
        var eventListQueryable = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Users = [
                    user
                ],
                EventUsers = [
                    new() { UserId = command.UserId, RegistrationDate = new DateOnly()}
                ]
            },
        }.BuildMock();

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _eventUserRepositoryMock.Setup(x =>
                x.GetAllEventsFromUser(
                    It.Is<Guid>(g => g == command.UserId)))
            .Returns(eventListQueryable);

        var handler = new GetAllUserEventsHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
    }
}