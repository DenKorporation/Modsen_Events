namespace Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set;  }
    public string Address { get; set; }
    // maybe it makes sense to do data type / enum instead of string
    public string Category { get; set; }
    public uint Capacity { get; set; }
    // url or local storage path
    public string? ImagePath { get; set; }
    public ICollection<User> Users { get; set; } = null!;
    public ICollection<EventUser> EventUsers { get; set; } = null!;
}