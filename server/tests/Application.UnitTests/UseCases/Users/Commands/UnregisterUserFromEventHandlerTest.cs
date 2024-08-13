using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Users.Commands.UnregisterUserFromEvent;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands;

[TestSubject(typeof(UnregisterUserFromEventHandler))]
public class UnregisterUserFromEventHandlerTest
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public UnregisterUserFromEventHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new UnregisterUserFromEventCommand(Guid.NewGuid(), Guid.NewGuid());

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new UnregisterUserFromEventHandler(
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
    public async Task Handle_EventNotExist_ReturnsEventNotFoundError()
    {
        // Arrange
        var command = new UnregisterUserFromEventCommand(Guid.NewGuid(), Guid.NewGuid());
        var dbUser = new User { Id = command.UserId };
        
        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);
        
        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event)null!);

        var handler = new UnregisterUserFromEventHandler(
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.UserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_UserNotRegisteredToEvent_ReturnsEventUserNotFoundError()
    {
        // Arrange
        var command = new UnregisterUserFromEventCommand(Guid.NewGuid(), Guid.NewGuid());
        var dbUser = new User { Id = command.UserId };
        var dbEvent = new Event { Id = command.EventId, Capacity = 0};
        
        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);
        
        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(
                x => x.RemoveUserFromEventAsync(
                    dbUser.Id,
                    dbEvent.Id,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UnregisterUserFromEventHandler(
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventUserNotFoundError>();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.UserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.RemoveUserFromEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new UnregisterUserFromEventCommand(Guid.NewGuid(), Guid.NewGuid());
        var dbUser = new User { Id = command.UserId };
        var dbEvent = new Event { Id = command.EventId, Capacity = 1};
        
        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);
        
        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(
                x => x.RemoveUserFromEventAsync(
                    dbUser.Id,
                    dbEvent.Id,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _unitOfWorkMock.Setup(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));

        var handler = new UnregisterUserFromEventHandler(
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.UserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.RemoveUserFromEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.RemoveUserFromEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SuccessfulCancellationOfRegistration_ReturnsOk()
    {
        // Arrange
        var command = new UnregisterUserFromEventCommand(Guid.NewGuid(), Guid.NewGuid());
        var dbUser = new User { Id = command.UserId };
        var dbEvent = new Event { Id = command.EventId, Capacity = 1};
        
        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);
        
        _eventRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbEvent);

        _eventUserRepositoryMock.Setup(
                x => x.RemoveUserFromEventAsync(
                    dbUser.Id,
                    dbEvent.Id,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _unitOfWorkMock.Setup(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        var handler = new UnregisterUserFromEventHandler(
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.UserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventRepositoryMock.Verify(
            x => x.GetByIdAsync(
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.RemoveUserFromEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.RemoveUserFromEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}