namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateOnly Birthday { get; set; }
    public ICollection<Event> Events { get; set; } = null!;
    public ICollection<EventUser> EventUsers { get; set; } = null!;
}