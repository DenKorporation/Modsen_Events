using Application.Common.Errors.Base;
using Application.UseCases.Events.Commands.CreateEvent;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using Moq;

namespace Application.UnitTests.UseCases.Events.Commands;

[TestSubject(typeof(CreateEventHandler))]
public class CreateEventHandlerTest
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public CreateEventHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new CreateEventCommand("test", "test", DateTime.Now, "test", "test", 1);

        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));

        var handler = new CreateEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mapperMock.Verify(
            x => x.Map<Event>(command),
            Times.Once);
        _mapperMock.Verify(
            x => x.Map<EventResponse>(
                It.IsAny<Event>()),
            Times.Never);
        _eventRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Event>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var command = new CreateEventCommand("test", "test", DateTime.Now, "test", "test", 1);

        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        var handler = new CreateEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mapperMock.Verify(
            x => x.Map<Event>(command),
            Times.Once);
        _mapperMock.Verify(
            x => x.Map<EventResponse>(
                It.IsAny<Event>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Event>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}