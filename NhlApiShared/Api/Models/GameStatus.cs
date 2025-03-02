﻿using Newtonsoft.Json;


namespace NhlApiShared.Api.Models
{
	//code	"1"
	//abstractGameState	"Preview"
	//detailedState	"Scheduled"
	//baseballCode	"S"
	//startTimeTBD	false
	//code	"2"
	//abstractGameState	"Preview"
	//detailedState	"Pre-Game"
	//baseballCode	"P"
	//startTimeTBD	false
	//code	"3"
	//abstractGameState	"Live"
	//detailedState	"In Progress"
	//baseballCode	"I"
	//startTimeTBD	false
	//code	"4"
	//abstractGameState	"Live"
	//detailedState	"In Progress - Critical"
	//baseballCode	"I"
	//startTimeTBD	false
	//code	"5"
	//abstractGameState	"Final"
	//detailedState	"Game Over"
	//baseballCode	"O"
	//startTimeTBD	false
	//code	"6"
	//abstractGameState	"Final"
	//detailedState	"Final"
	//baseballCode	"F"
	//startTimeTBD	false
	//code	"7"
	//abstractGameState	"Final"
	//detailedState	"Final"
	//baseballCode	"F"
	//startTimeTBD	false
	//code	"8"
	//abstractGameState	"Preview"
	//detailedState	"Scheduled (Time TBD)"
	//baseballCode	"S"
	//startTimeTBD	false
	//code	"9"
	//abstractGameState	"Preview"
	//detailedState	"Postponed"
	//baseballCode	"S"
	//startTimeTBD	false

	/// <summary>
	/// https://statsapi.web.nhl.com/api/v1/gameStatus
	/// </summary>
	public class GameStatus
	{
		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("abstractGameState")]
		public string AbstractGameState { get; set; }

		[JsonProperty("detailedState")]
		public string DetailedState { get; set; }

		[JsonProperty("baseballCode")]
		public string BaseballCode { get; set; }

		[JsonProperty("startTimeTBD")]
		public bool StartTimeTBD { get; set; }
	}
}