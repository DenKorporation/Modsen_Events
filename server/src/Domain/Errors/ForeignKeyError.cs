using FluentResults;

namespace Domain.Errors;

public class ForeignKeyError(string message, string table, string foreignTable) : Error(message)
{
    public string Table { get; set; } = table;
    public string ForeignTable { get; set; } = foreignTable;
}