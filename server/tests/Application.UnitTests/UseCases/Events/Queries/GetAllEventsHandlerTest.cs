using Application.UseCases.Events.Queries.GetAllEvents;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Events.Queries;

[TestSubject(typeof(GetAllEventsHandler))]
public class GetAllEventsHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetAllEventsMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public GetAllEventsHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_NonEmptyCollection_ReturnsOkWithPagedList()
    {
        // Arrange
        var command = new GetAllEventsQuery(1, 5);
        var eventListQueryable = new List<Event>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Users = []
            },
        }.BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetAllAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventListQueryable);

        var handler = new GetAllEventsHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
    }
}