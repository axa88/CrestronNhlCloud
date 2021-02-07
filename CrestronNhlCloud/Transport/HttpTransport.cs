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