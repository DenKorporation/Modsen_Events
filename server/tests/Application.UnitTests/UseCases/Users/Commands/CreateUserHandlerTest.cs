using Application.Common.Errors;
using Application.Common.Errors.Base;
using Application.UseCases.Users.Commands.CreateUser;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using FluentResults;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands;

[TestSubject(typeof(CreateUserHandler))]
public class CreateUserHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<CreateUserMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock = GetMockUserManager();


    public CreateUserHandlerTest()
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
    public async Task Handle_DuplicateEmail_ReturnsConflictError()
    {
        // Arrange
        var command = new CreateUserCommand("firstName", "lastName", "email", "password", "1111-11-11");

        _userRepositoryMock.Setup(x =>
                x.CreateAsync(
                    It.IsAny<User>(),
                    command.Password,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail" }));

        var handler = new CreateUserHandler(
            _mapperMock,
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ConflictError>();
        _userRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                command.Password,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("PasswordTooShort")]
    [InlineData("PasswordRequiresUpper")]
    [InlineData("PasswordRequiresLower")]
    [InlineData("PasswordRequiresNonAlphanumeric")]
    [InlineData("PasswordRequiresDigit")]
    [InlineData("PasswordRequiresUniqueChars")]
    public async Task Handle_InvalidPassword_ReturnsValidationError(string errorCode)
    {
        // Arrange
        var command = new CreateUserCommand("firstName", "lastName", "email", "invalid", "1111-11-11");

        _userRepositoryMock.Setup(x =>
                x.CreateAsync(
                    It.IsAny<User>(),
                    command.Password,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = errorCode }));

        var handler = new CreateUserHandler(
            _mapperMock,
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ValidationError>();
        _userRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                command.Password,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var command = new CreateUserCommand("firstName", "lastName", "email", "password", "1111-11-11");

        _userRepositoryMock.Setup(x =>
                x.CreateAsync(
                    It.IsAny<User>(),
                    command.Password,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "something went wrong" }));

        var handler = new CreateUserHandler(
            _mapperMock,
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _userRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                command.Password,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var command = new CreateUserCommand("firstName", "lastName", "email", "password", "1111-11-11");

        _userRepositoryMock.Setup(x =>
                x.CreateAsync(
                    It.IsAny<User>(),
                    command.Password,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(
                x => x.AddToRoleAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new CreateUserHandler(
            _mapperMock,
            _unitOfWorkMock.Object,
            _userManagerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}