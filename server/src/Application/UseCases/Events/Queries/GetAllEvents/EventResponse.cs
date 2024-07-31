namespace Application.UseCases.Events.Queries.GetAllEvents;

public record EventResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Address { get; set; }
    public string Category { get; set; }
    public uint Capacity { get; set; }
    public string? ImageUrl { get; set; }
    public uint PlacesOccupied { get; set; }
}