using Application.UseCases.Events.Queries.GetFilteredEvents;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Events.Queries;

[TestSubject(typeof(GetFilteredEventsHandler))]
public class GetFilteredEventsHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetFilteredEventsMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public GetFilteredEventsHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_NonEmptyCollection_ReturnsOkWithPagedList()
    {
        // Arrange
        var command = new GetFilteredEventsQuery(1, 5, null, null, null, null, null);
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

        var handler = new GetFilteredEventsHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
    }
    
    [Theory]
    [InlineData("name", null, null, 2)]
    [InlineData(null, "address", null, 2)]
    [InlineData(null, null, "category", 2)]
    [InlineData("name", "12345", "category", 1)]
    public async Task Handle_FilterByStringField_ReturnsOkWithPagedListOfFilteredEvent(string? name, string? address, string? category, int expected)
    {
        // Arrange
        var command = new GetFilteredEventsQuery(1, 5, name, address, category, null, null);
        var eventListQueryable = new List<Event>
        {
            new()
            {
                Name = "name",
                Address = "address",
                Category = "category",
                Id = Guid.NewGuid(),
                Users = []
            },
            new()
            {
                Name = "name12345",
                Address = "address12345",
                Category = "category12345",
                Id = Guid.NewGuid(),
                Users = []
            },
            new()
            {
                Name = "12345",
                Address = "12345",
                Category = "12345",
                Id = Guid.NewGuid(),
                Users = []
            },
        }.BuildMock();

        _eventRepositoryMock.Setup(x =>
                x.GetAllAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventListQueryable);

        var handler = new GetFilteredEventsHandler(
            _mapperMock,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(expected);
    }
}