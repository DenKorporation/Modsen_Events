using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Events.Commands.DeleteEvent;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using Moq;

namespace Application.UnitTests.UseCases.Events.Commands;

[TestSubject(typeof(DeleteEventHandler))]
public class DeleteEventHandlerTest
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public DeleteEventHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_EventNotExist_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = new DeleteEventCommand(Guid.NewGuid());

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null!);

        var handler = new DeleteEventHandler(
            _mapperMock.Object,
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
    public async Task Handle_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new DeleteEventCommand(Guid.NewGuid());
        var dbEvent = new Event { Id = command.EventId };

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));

        var handler = new DeleteEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.DeleteAsync(
                dbEvent,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteSuccess_ReturnsOk()
    {
        // Arrange
        var command = new DeleteEventCommand(Guid.NewGuid());
        var dbEvent = new Event { Id = command.EventId };

        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        var handler = new DeleteEventHandler(
            _mapperMock.Object,
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.DeleteAsync(
                dbEvent,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}