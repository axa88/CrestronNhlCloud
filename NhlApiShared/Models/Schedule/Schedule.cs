using System;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace NhlApi.Models.Schedule
{
	public class Schedule
	{
		[JsonProperty(PropertyName = "copyright")]
		public string Copyright { get; set; }

		[JsonProperty(PropertyName = "totalItems")]
		public int TotalItems { get; set; }

		[JsonProperty(PropertyName = "totalEvents")]
		public int TotalEvents { get; set; }

		[JsonProperty(PropertyName = "totalGames")]
		public int TotalGames { get; set; }

		[JsonProperty(PropertyName = "totalMatches")]
		public int TotalMatches { get; set; }

		[JsonProperty(PropertyName = "wait")]
		public int Wait { get; set; }

		[JsonProperty(PropertyName = "dates")]
		public List<Date> Dates { get; set; }
	}

	public class Date
	{
		[JsonProperty(PropertyName = "date")]
		public string DateSimple { get; set; }

		[JsonProperty(PropertyName = "totalItems")]
		public int TotalItems { get; set; }

		[JsonProperty(PropertyName = "totalEvents")]
		public int TotalEvents { get; set; }

		[JsonProperty(PropertyName = "totalGames")]
		public int TotalGames { get; set; }

		[JsonProperty(PropertyName = "totalMatches")]
		public int TotalMatches { get; set; }

		[JsonProperty(PropertyName = "games")]
		public List<Game> Games { get; set; }

		[JsonProperty(PropertyName = "events")]
		public List<object> Events { get; set; }

		[JsonProperty(PropertyName = "matches")]
		public List<object> Matches { get; set; }
	}

	public class Game
	{
		[JsonProperty(PropertyName = "gamePk")]
		public int GamePk { get; set; }

		[JsonProperty(PropertyName = "link")]
		public string Link { get; set; }

		[JsonProperty(PropertyName = "gameType")]
		public string GameType { get; set; }

		[JsonProperty(PropertyName = "season")]
		public string Season { get; set; }

		[JsonProperty(PropertyName = "gameDate")]
		public DateTime GameDate { get; set; }

		[JsonProperty(PropertyName = "status")]
		public Status Status { get; set; }

		[JsonProperty(PropertyName = "teams")]
		public Teams Teams { get; set; }

		[JsonProperty(PropertyName = "venue")]
		public Venue Venue { get; set; }

		[JsonProperty(PropertyName = "content")]
		public Content Content { get; set; }
	}

	public class Status
	{
		[JsonProperty(PropertyName = "abstractGameState")]
		public string AbstractGameState { get; set; }

		[JsonProperty(PropertyName = "codedGameState")]
		public string CodedGameState { get; set; }

		[JsonProperty(PropertyName = "detailedState")]
		public string DetailedState { get; set; }

		[JsonProperty(PropertyName = "statusCode")]
		public string StatusCode { get; set; }

		[JsonProperty(PropertyName = "startTimeTBD")]
		public bool StartTimeTbd { get; set; }
	}

	public class Teams
	{
		[JsonProperty(PropertyName = "away")]
		public HomeAway Away { get; set; }

		[JsonProperty(PropertyName = "home")]
		public HomeAway Home { get; set; }
	}

	public class Team
	{
		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "link")]
		public string Link { get; set; }
	}

	public class Venue
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "link")]
		public string Link { get; set; }
	}

	public class Content
	{
		[JsonProperty(PropertyName = "link")]
		public string Link { get; set; }
	}

	public class LeagueRecord
	{
		[JsonProperty(PropertyName = "wins")]
		public int Wins { get; set; }

		[JsonProperty(PropertyName = "losses")]
		public int Losses { get; set; }

		[JsonProperty(PropertyName = "ot")]
		public int Ot { get; set; }

		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }
	}

	public class HomeAway
	{
		[JsonProperty(PropertyName = "leagueRecord")]
		public LeagueRecord LeagueRecord { get; set; }

		[JsonProperty(PropertyName = "score")]
		public int Score { get; set; }

		[JsonProperty(PropertyName = "team")]
		public Team Team { get; set; }
	}
}