namespace Application.UseCases.Users.Commands.CreateUser;

public record UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateOnly Birthday { get; set; }
}