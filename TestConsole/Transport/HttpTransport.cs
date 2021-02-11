using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Api.Models.Schedule;
using Api.Models.Teams;

using Newtonsoft.Json;


namespace TestConsole.Transport
{
	public class HttpTransport : IDisposable
	{
		private readonly HttpClient _httpClient;

		public HttpTransport()
		{
			_httpClient = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(3)
			};
		}

		public void Dispose()
		{
			_httpClient.CancelPendingRequests();
			_httpClient.Dispose();
		}

		public ushort Timeout
		{
			set => _httpClient.Timeout = TimeSpan.FromSeconds(value);
		}

		public string SetBaseUrl { private get; set; } = "https://statsapi.web.nhl.com/api/v1/";

		public List<Api.Models.Teams.Team> GetTeams()
		{
			var uri = new Uri($"{SetBaseUrl}teams");
			var httpClientRequest = new HttpRequestMessage(HttpMethod.Get, uri);

			httpClientRequest.Headers.Add("Accept", "application/json");
			httpClientRequest.Headers.Add("User-Agent", "NhlUpdate-0.0.0");

			var response = _httpClient.SendAsync(httpClientRequest).Result;
			if (response.IsSuccessStatusCode)
			{
				var s = response.Content.ReadAsStringAsync().Result;
				return JsonConvert.DeserializeObject<TeamsRoot>(s).Teams;
			}

			return default;
		}

		public Schedule GetTeamSchedule(ushort teamId)
		{
			var uri = new Uri($"{SetBaseUrl}schedule?teamId={teamId}");
			var httpClientRequest = new HttpRequestMessage(HttpMethod.Get, uri);
			httpClientRequest.Headers.Add("Accept", "application/json");
			httpClientRequest.Headers.Add("User-Agent", "NhlUpdate-0.0.0");

			var response = _httpClient.SendAsync(httpClientRequest).Result;
			if (response.IsSuccessStatusCode)
			{
				var s = response.Content.ReadAsStringAsync().Result;
				return JsonConvert.DeserializeObject<Schedule>(s);
			}

			return default;
		}
	}
}