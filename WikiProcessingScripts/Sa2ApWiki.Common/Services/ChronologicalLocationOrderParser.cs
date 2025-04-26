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

			var orderTextSplit = orderText.Split(',').Select(y => y.Trim());

			var chronologicalLocationModels = new List<ChronologicalLocationModel>();
			foreach (var singleLocation in orderTextSplit)
			{
				// TODO: remove the filtering here
				if (!singleLocation.Contains("pipe") && !singleLocation.Contains("hidden"))
				{
					continue;
				}
				
				var splitByPlusSign = singleLocation.Split('+').Select(x => x.Trim()).ToImmutableArray();

				var firstLocation = splitByPlusSign[0];
				var secondLocation = (string?)null;

				if (splitByPlusSign.Length > 1)
				{
					secondLocation = splitByPlusSign[1];
				}
				
				var chronologicalLocationModel = new ChronologicalLocationModel
				{
					FirstLocationName = firstLocation,
					SecondLocationName = secondLocation,
					NextLocation = null
				};

				if (chronologicalLocationModels.Count > 0)
				{
					chronologicalLocationModels.Last().NextLocation = chronologicalLocationModel;
				}
				
				chronologicalLocationModels.Add(chronologicalLocationModel);
			}
			
			chronologicalLocationsByStage.Add(stageName, chronologicalLocationModels);
		}

		return chronologicalLocationsByStage;
	}
}