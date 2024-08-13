using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Users.Commands.AssignRoleToUser;
using Domain.Constants;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using IdentityModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands;

[TestSubject(typeof(AssignRoleToUserHandler))]
public class AssignRoleToUserHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock = GetMockUserManager();


    public AssignRoleToUserHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }
    
    private static Mock<UserManager<User>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object, 
            null, 
            null, 
            null, 
            null, 
            null, 
            null, 
            null, 
            null);
    }
    
    [Fact]
    public async Task Handle_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new AssignRoleToUserCommand(Guid.NewGuid(), Roles.Administrator);

        _userManagerMock.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        var handler = new AssignRoleToUserHandler(
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _userManagerMock.Verify(
            x => x.FindByIdAsync(
                command.UserId.ToString()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_UserAlreadyHasThisRole_ReturnsConflictError()
    {
        // Arrange
        var command = new AssignRoleToUserCommand(Guid.NewGuid(), Roles.Administrator);
        var dbUser = new User { Id = command.UserId };

        _userManagerMock.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(dbUser);

        _userRepositoryMock.Setup(
                x => x.IsInRoleAsync(
                    dbUser,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new AssignRoleToUserHandler(
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ConflictError>();
        _userManagerMock.Verify(
            x => x.FindByIdAsync(
                command.UserId.ToString()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_AssignRoleFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new AssignRoleToUserCommand(Guid.NewGuid(), Roles.Administrator);
        var dbUser = new User { Id = command.UserId };

        _userManagerMock.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(dbUser);

        _userRepositoryMock.Setup(
                x => x.IsInRoleAsync(
                    dbUser,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(
                x => x.AssignRoleAsync(
                    dbUser,
                    command.Role,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed());

        var handler = new AssignRoleToUserHandler(
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _userManagerMock.Verify(
            x => x.FindByIdAsync(
                command.UserId.ToString()),
            Times.Once);
        _userRepositoryMock.Verify(
            x => x.AssignRoleAsync(
                dbUser,
                command.Role,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SuccessfulRoleAssignment_ReturnsOk()
    {
        // Arrange
        var command = new AssignRoleToUserCommand(Guid.NewGuid(), Roles.Administrator);
        var dbUser = new User { Id = command.UserId };

        _userManagerMock.Setup(
                x => x.FindByIdAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(dbUser);

        _userRepositoryMock.Setup(
                x => x.IsInRoleAsync(
                    dbUser,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(
                x => x.AssignRoleAsync(
                    dbUser,
                    command.Role,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        
        _userManagerMock.Setup(
                x => x.GetClaimsAsync(
                    dbUser))
            .ReturnsAsync([new Claim(JwtClaimTypes.UpdatedAt, "old")]);
        
        _userManagerMock.Setup(
                x => x.ReplaceClaimAsync(
                    dbUser,
                    It.IsAny<Claim>(),
                    It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        _unitOfWorkMock.Setup(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok);

        var handler = new AssignRoleToUserHandler(
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userManagerMock.Verify(
            x => x.FindByIdAsync(
                command.UserId.ToString()),
            Times.Once);
        _userRepositoryMock.Verify(
            x => x.AssignRoleAsync(
                dbUser,
                command.Role,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}