using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Users.Commands.RegisterUserToEvent;
using AutoMapper;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands;

[TestSubject(typeof(RegisterUserToEventHandler))]
public class RegisterUserToEventHandlerTest
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public RegisterUserToEventHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new RegisterUserToEventCommand(Guid.NewGuid(), Guid.NewGuid());

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new RegisterUserToEventHandler(
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
        var command = new RegisterUserToEventCommand(Guid.NewGuid(), Guid.NewGuid());
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

        var handler = new RegisterUserToEventHandler(
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
    public async Task Handle_EventCapacityConflict_ReturnsConflictError()
    {
        // Arrange
        var command = new RegisterUserToEventCommand(Guid.NewGuid(), Guid.NewGuid());
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
                x => x.GetAllUsersFromEvent(
                    dbEvent.Id))
            .Returns(new List<User>().BuildMock());

        var handler = new RegisterUserToEventHandler(
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ConflictError>();
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
            x => x.GetAllUsersFromEvent(
                command.EventId),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_UserAlreadyRegisteredToThisEvent_ReturnsConflictError()
    {
        // Arrange
        var command = new RegisterUserToEventCommand(Guid.NewGuid(), Guid.NewGuid());
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
                x => x.GetAllUsersFromEvent(
                    dbEvent.Id))
            .Returns(new List<User>().BuildMock());
        
        _unitOfWorkMock.Setup(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new PrimaryKeyError("message", "table")));

        var handler = new RegisterUserToEventHandler(
            _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ConflictError>();
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
            x => x.GetAllUsersFromEvent(
                command.EventId),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.AddUserToEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsOk()
    {
        // Arrange
        var command = new RegisterUserToEventCommand(Guid.NewGuid(), Guid.NewGuid());
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
                x => x.GetAllUsersFromEvent(
                    dbEvent.Id))
            .Returns(new List<User>().BuildMock());
        
        _unitOfWorkMock.Setup(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        var handler = new RegisterUserToEventHandler(
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
            x => x.GetAllUsersFromEvent(
                command.EventId),
            Times.Once);
        _eventUserRepositoryMock.Verify(
            x => x.AddUserToEventAsync(
                command.UserId,
                command.EventId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}