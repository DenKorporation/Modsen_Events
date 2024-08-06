using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly Birthday { get; set; }
    // in order to validate the token
    public DateTime UpdatedAt { get; set; }
    public ICollection<Event> Events { get; set; } = null!;
    public ICollection<EventUser> EventUsers { get; set; } = null!;
}