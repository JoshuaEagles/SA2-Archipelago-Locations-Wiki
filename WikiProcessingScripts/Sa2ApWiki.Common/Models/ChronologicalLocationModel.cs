namespace Sa2ApWiki.Common.Models;

public record ChronologicalLocationModel
{
	public required string FirstLocationName { get; init; }
	public string? SecondLocationName { get; init; }
	
	// sucks to make this mutable but this is a nice quick fix
	// don't mutate this outside of ChronologicalLocationOrderParser
	public required ChronologicalLocationModel? NextLocation { get; set; }
}