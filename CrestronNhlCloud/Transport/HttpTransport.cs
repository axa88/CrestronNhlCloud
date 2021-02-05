using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NhlApi.Models.Schedule;
using NhlApi.Models.Teams;

using Team = NhlApi.Models.Teams.Team;


namespace CrestronNhlCloud.Transport
{
	internal class HttpApiTransport : IDisposable
	{
		private readonly HttpClient _httpClient;
		private readonly string _baseUrl = "http://statsapi.web.nhl.com/api/v1/";

		internal HttpApiTransport()
		{
			_httpClient = new HttpClient
			{
				KeepAlive = false,
				Timeout = 3,
				TimeoutEnabled = true,
			};
		}

		public void Dispose()
		{
			_httpClient.Abort();
			_httpClient.Dispose();
		}

		internal string SendSync()
		{
			CrestronConsole.PrintLine("sending");
			var httpClientRequest = new HttpClientRequest { RequestType = RequestType.Get };
			var requestHeaders = new[]
			{
				//new HttpHeader("Cache-Control", "no-cache"),
				//new HttpHeader("Connection", "close"),
				new HttpHeader("Accept", "*/*"),
				//new HttpHeader("Accept", "application/json"),
				//new HttpHeader("Pragma", "no-cache"),
				//new HttpHeader("X-Auth-PSK", psk),
				new HttpHeader("User-Agent", "ChNhlGoalLight-0.0.0)")
			};

			foreach (var httpHeader in requestHeaders)
				httpClientRequest.Header.AddHeader(httpHeader);
			//_httpClientRequest.Url.Parse("http://" + ipAddress + ":" + port + "/jsonrpc");
			httpClientRequest.Url.Parse(_baseUrl + "schedule?teamId=5");
			CrestronConsole.PrintLine("sent");

			try
			{
				CrestronConsole.PrintLine("trying");

				//_httpClient.KeepAlive = false;
				//_httpClient.Accept = "application/json";
				//_httpClient.EnableNagle = true;

				var response = _httpClient.Dispatch(httpClientRequest);
				if (response.Code == 200 && response.Header.ContentType.Contains("application/json"))
				{
					CrestronConsole.PrintLine("receiving");

					var s = Encoding.UTF8.GetString(response.ContentBytes, 0, response.ContentBytes.Length);
					CrestronConsole.PrintLine("encoded");

					var schedule = JsonConvert.DeserializeObject<Schedule>(s);
					CrestronConsole.PrintLine("deserd");
					CrestronConsole.PrintLine(schedule.TotalGames.ToString());
					return schedule.TotalGames > 0 ? schedule.Dates.First().Games.First().Teams.Home.Team.Name : "no game today";
				}
				else
					CrestronConsole.PrintLine(response.Code.ToString());
			}
			catch (Exception exception)
			{
				CrestronConsole.PrintLine("some throw");
				CrestronConsole.PrintLine(exception.Message);
			}

			return "bad";
		}

		public List<Team> GetTeams()
		{
			var httpClientRequest = new HttpClientRequest();
			httpClientRequest.Url.Parse($"{_baseUrl}teams");
			httpClientRequest.Header.AddHeader(new HttpHeader("Accept", "application/json"));
			httpClientRequest.Header.AddHeader(new HttpHeader("User-Agent", "NhlUpdate-0.0.0"));

			var response = _httpClient.Dispatch(httpClientRequest);
			if (response.Code == 200 && response.Header.ContentType.Contains("application/json"))
			{
				var s = Encoding.UTF8.GetString(response.ContentBytes, 0, response.ContentBytes.Length);
				return JsonConvert.DeserializeObject<TeamsRoot>(s).Teams;
			}

			return default;
		}

		public Schedule GetTeamSchedule(ushort teamId)
		{
			var httpClientRequest = new HttpClientRequest();
			httpClientRequest.Url.Parse($"{_baseUrl}schedule?teamId={teamId}");
			httpClientRequest.Header.AddHeader(new HttpHeader("Accept", "application/json"));
			httpClientRequest.Header.AddHeader(new HttpHeader("User-Agent", "NhlUpdate-0.0.0"));

			var response = _httpClient.Dispatch(httpClientRequest);
			if (response.Code == 200 && response.Header.ContentType.Contains("application/json"))
			{
				var s = Encoding.UTF8.GetString(response.ContentBytes, 0, response.ContentBytes.Length);
				return JsonConvert.DeserializeObject<Schedule>(s);
			}

			return default;
		}
	}
}