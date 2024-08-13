using Application.Common.Errors;
using Application.UseCases.Users.Queries.GetUserById;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using JetBrains.Annotations;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.UseCases.Users.Queries;

[TestSubject(typeof(GetUserByIdHandler))]
public class GetUserByIdHandlerTest
{
    private readonly IMapper _mapperMock =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetUserByIdMappingProfile>()));

    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly Mock<IEventUserRepository> _eventUserRepositoryMock = new();

    public GetUserByIdHandlerTest()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.EventUserRepository).Returns(_eventUserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var command = new GetUserByIdQuery(Guid.NewGuid());

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var handler = new GetUserByIdHandler(
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
    public async Task Handle_UserExist_ReturnsOkWithUser()
    {
        // Arrange
        var command = new GetUserByIdQuery(Guid.NewGuid());
        var dbUser = new User()
        {
            Id = command.UserId
        };
        var userListQueryable = new List<User>().BuildMock();

        _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbUser);

        var handler = new GetUserByIdHandler(
            _mapperMock,
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
    }
}