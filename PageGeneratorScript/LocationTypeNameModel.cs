namespace ChronologicalDataCollectionScript;

public record LocationTypeNameModel
{
	public required string CodeName { get; init; }
	public required string ReadableName { get; init; }
}