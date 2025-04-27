using Sa2ApWiki.Common.Models;

namespace Sa2ApWiki.Common;

public static class Constants
{
	public static readonly string[] CharacterNames = [
		"Sonic",
		"Tails",
		"Knuckles",
		"Rouge",
		"Eggman",
		"Shadow",
		"CannonsCore"
	];

	public static readonly LocationTypeNameModel[] LocationTypes = [
		new() { CodeName = "chaobox", ReadableName = "Chaobox" },
		new() { CodeName = "pipe", ReadableName = "Pipe" },
		new() { CodeName = "hidden", ReadableName = "Hidden" },
		new() { CodeName = "goldbeetle", ReadableName = "Gold Beetle" },
		new() { CodeName = "omochao", ReadableName = "Omochao" },
		new() { CodeName = "animal", ReadableName = "Animal" },
		new() { CodeName = "item", ReadableName = "Item" },
		new() { CodeName = "life", ReadableName = "Life" },
		new() { CodeName = "big", ReadableName = "Big" }
	];

	public static readonly Dictionary<string, string> StageCodeNameToReadableName = new()
	{
		// Hero Story
		{"CityEscape", "City Escape"},
		{"WildCanyon", "Wild Canyon"},
		{"PrisonLane", "Prison Lane"},
		{"MetalHarbor", "Metal Harbor"},
		{"GreenForest", "Green Forest"},
		{"PumpkinHill", "Pumpkin Hill"},
		{"MissionStreet", "Mission Street"},
		{"AquaticMine", "Aquatic Mine"},
		{"Route101", "Route 101"},
		{"HiddenBase", "Hidden Base"},
		{"PyramidCave", "Pyramid Cave"},
		{"DeathChamber", "Death Chamber"},
		{"EternalEngine", "Eternal Engine"},
		{"MeteorHerd", "Meteor Herd"},
		{"CrazyGadget", "Crazy Gadget"},
		{"FinalRush", "Final Rush"},
    
		// Dark Story
		{"IronGate", "Iron Gate"},
		{"DryLagoon", "Dry Lagoon"},
		{"SandOcean", "Sand Ocean"},
		{"RadicalHighway", "Radical Highway"},
		{"EggQuarters", "Egg Quarters"},
		{"LostColony", "Lost Colony"},
		{"WeaponsBed", "Weapons Bed"},
		{"SecurityHall", "Security Hall"},
		{"WhiteJungle", "White Jungle"},
		{"Route280", "Route 280"},
		{"SkyRail", "Sky Rail"},
		{"MadSpace", "Mad Space"},
		{"CosmicWall", "Cosmic Wall"},
		{"FinalChase", "Final Chase"},
    
		// Last Story
		{"CannonsCore", "Cannons Core"},
		
		{"GreenHill", "Green Hill"}
	};

	public static readonly IReadOnlyCollection<string> StageNamesInOrder =
	[
		// Hero Story
		"CityEscape",
		"WildCanyon",
		"PrisonLane",
		"MetalHarbor",
		"GreenForest",
		"PumpkinHill",
		"MissionStreet",
		"AquaticMine",
		"Route101",
		"HiddenBase",
		"PyramidCave",
		"DeathChamber",
		"EternalEngine",
		"MeteorHerd",
		"CrazyGadget",
		"FinalRush",

		// Dark Story
		"IronGate",
		"DryLagoon",
		"SandOcean",
		"RadicalHighway",
		"EggQuarters",
		"LostColony",
		"WeaponsBed",
		"SecurityHall",
		"WhiteJungle",
		"Route280",
		"SkyRail",
		"MadSpace",
		"CosmicWall",
		"FinalChase",

		// Last Story
		"CannonsCore",

		"GreenHill"
	];
}
