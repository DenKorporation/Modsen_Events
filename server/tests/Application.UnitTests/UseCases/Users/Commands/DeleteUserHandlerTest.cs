using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Users.Commands.DeleteUser;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands;

[TestSubject(typeof(DeleteUserHandler))]
public class DeleteUserHandlerTest
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public DeleteUserHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new DeleteUserHandler(
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
    public async Task Handle_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());
        var dbUser = new User { Id = command.UserId };

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);

        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(""));

        var handler = new DeleteUserHandler(
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
        _userRepositoryMock.Verify(
            x => x.DeleteAsync(
                dbUser,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteSuccess_ReturnsOk()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());
        var dbUser = new User() { Id = command.UserId };

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);

        _unitOfWorkMock.Setup(x =>
                x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        var handler = new DeleteUserHandler(
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
        _userRepositoryMock.Verify(
            x => x.DeleteAsync(
                dbUser,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}