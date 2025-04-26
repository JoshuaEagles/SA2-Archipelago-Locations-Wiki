namespace Sa2ApWiki.Common.Models;

public record LocationTypeNameModel
{
	public required string CodeName { get; init; }
	public required string ReadableName { get; init; }
}