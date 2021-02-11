using System.Collections.Generic;

using Newtonsoft.Json;


namespace Api.Models.Teams
{
	public enum Tz { Cst, Est, Mst, Pst };
	public enum Division { DiscoverCentral, HondaWest, MassMutualEast, ScotiaNorth };
	public enum Conference { Eastern, Western };

	public class TeamsRoot
	{
		[JsonProperty("copyright")]
		public string Copyright { get; set; }

		[JsonProperty("teams")]
		public List<Team> Teams { get; set; }
	}

	public class Team
	{
		[JsonProperty("id")]
		public ushort Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }

		[JsonProperty("venue")]
		public Venue Venue { get; set; }

		[JsonProperty("abbreviation")]
		public string Abbreviation { get; set; }

		[JsonProperty("teamName")]
		public string TeamName { get; set; }

		[JsonProperty("locationName")]
		public string LocationName { get; set; }

		[JsonProperty("firstYearOfPlay")]
		public string FirstYearOfPlay { get; set; }

		[JsonProperty("division")]
		public Section Division { get; set; }

		[JsonProperty("conference")]
		public Section Conference { get; set; }

		[JsonProperty("franchise")]
		public Franchise Franchise { get; set; }

		[JsonProperty("shortName")]
		public string ShortName { get; set; }

		[JsonProperty("officialSiteUrl")]
		public string OfficialSiteUrl { get; set; }

		[JsonProperty("franchiseId")]
		public ushort FranchiseId { get; set; }

		[JsonProperty("active")]
		public bool Active { get; set; }
	}

	public class Venue
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("timeZone")]
		public TimeZone TimeZone { get; set; }

		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public ushort? Id { get; set; }
	}

	public class TimeZone
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("offset")]
		public short Offset { get; set; }

		[JsonProperty("tz")]
		public Tz Tz { get; set; }
	}


	public class Section
	{
		[JsonProperty("id")]
		public ushort Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }
	}

	public class Franchise
	{
		[JsonProperty("franchiseId")]
		public ushort FranchiseId { get; set; }

		[JsonProperty("teamName")]
		public string TeamName { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }
	}
}