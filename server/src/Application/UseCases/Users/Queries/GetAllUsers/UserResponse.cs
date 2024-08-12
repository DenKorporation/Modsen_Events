namespace Application.UseCases.Users.Queries.GetAllUsers;

public record UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateOnly Birthdate { get; set; }
}