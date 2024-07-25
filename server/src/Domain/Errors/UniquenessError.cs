using FluentResults;

namespace Domain.Errors;

public class UniquenessError(string message, string table, string column) : Error(message)
{
    public string Table { get; set; } = table;
    public string Column { get; set; } = column;
}