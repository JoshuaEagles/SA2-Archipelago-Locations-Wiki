namespace Sa2ApWiki.Common.Models;

public record ChronologicalLocationModel
{
	public required string LocationName { get; init; }
	public required ChronologicalLocationModel? NextLocation { get; init; }
}