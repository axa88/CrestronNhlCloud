using Newtonsoft.Json;


namespace NhlApiShared.Api.Models
{
	public class Season
	{
		[JsonProperty("seasonId")]
		public string SeasonId { get; set; }

		[JsonProperty("regularSeasonStartDate")]
		public string RegularSeasonStartDate { get; set; }

		[JsonProperty("regularSeasonEndDate")]
		public string RegularSeasonEndDate { get; set; }

		[JsonProperty("seasonEndDate")]
		public string SeasonEndDate { get; set; }

		[JsonProperty("numberOfGames")]
		public ushort NumberOfGames { get; set; }

		[JsonProperty("tiesInUse")]
		public bool TiesInUse { get; set; }

		[JsonProperty("olympicsParticipation")]
		public bool OlympicsParticipation { get; set; }

		[JsonProperty("conferencesInUse")]
		public bool ConferencesInUse { get; set; }

		[JsonProperty("divisionsInUse")]
		public bool DivisionsInUse { get; set; }

		[JsonProperty("wildCardInUse")]
		public bool WildCardInUse { get; set; }
	}
}
