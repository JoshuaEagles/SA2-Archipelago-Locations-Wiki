using System.Collections.Immutable;
using Sa2ApWiki.Common.Models;

namespace Sa2ApWiki.Common.Services;

public static class ChronologicalLocationOrderParser
{
	public static Dictionary<string, IReadOnlyCollection<ChronologicalLocationModel>> Parse(string path)
	{
		using var streamReader = new StreamReader(path);
		
		var lines = streamReader.ReadToEnd().Split('\n').Where(x => !string.IsNullOrWhiteSpace(x));

		var chronologicalLocationsByStage = new Dictionary<string, IReadOnlyCollection<ChronologicalLocationModel>>();
		foreach (var line in lines)
		{
			var indexOfFirstSpace = line.IndexOf(' ');
			var stageName = line.Substring(0, indexOfFirstSpace);
			var orderText = line.Substring(indexOfFirstSpace);

			var orderTextSplit = orderText.Trim().Split(' ').Select(y => y.Trim()).ToImmutableArray();

			var chronologicalLocationNames = new List<string>();
			foreach (var singleChronologicalEntry in orderTextSplit)
			{
				var splitByPlusSign = singleChronologicalEntry.Split('+').Select(x => x.Trim()).ToImmutableArray();

				var firstLocationName = splitByPlusSign[0];
				chronologicalLocationNames.Add(firstLocationName);

				if (splitByPlusSign.Length > 1)
				{
					var secondLocationName = splitByPlusSign[1];
					chronologicalLocationNames.Add(secondLocationName);
				}
			}

			// Iterate backwards so that we can reference the next location when creating a location
			// We can just reverse the list after to get the correct order
			// This means index 0 of the list will initially be the last location, index 1 the second last, etc
			var chronologicalLocationModels = new List<ChronologicalLocationModel>();
			for (var i = chronologicalLocationNames.Count - 1; i >= 0; i--)
			{
				var currentLocationName = chronologicalLocationNames[i];

				if (i == chronologicalLocationNames.Count - 1)
				{
					chronologicalLocationModels.Add(
						new ChronologicalLocationModel
						{
							LocationName = currentLocationName,
							NextLocation = null
						});
				}
				else
				{
					// This first reverses the index since i is starting from the end
					// Then it subtracts because the count is one more than the last index
					// Then it subtracts one again to get the previous location model that was inserted
					var nextLocationModel = chronologicalLocationModels[chronologicalLocationNames.Count - i - 1 - 1];
					
					chronologicalLocationModels.Add(new ChronologicalLocationModel
					{
						LocationName = currentLocationName,
						NextLocation = nextLocationModel
					});
				}

			}
			
			chronologicalLocationModels.Reverse();
			
			chronologicalLocationsByStage.Add(stageName, chronologicalLocationModels);
		}

		return chronologicalLocationsByStage;
	}
}