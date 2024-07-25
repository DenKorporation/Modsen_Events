using FluentResults;

namespace Domain.Errors;

public class PrimaryKeyError(string message, string table) : Error(message)
{
    public string Table { get; set; } = table;
}