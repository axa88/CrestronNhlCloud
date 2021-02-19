using System;
using System.Collections.Generic;
using System.Text;

using NhlApiShared.Api.Models.Schedule;
using NhlApiShared.Api.Models.Teams;

using Crestron.SimplSharp.Net.Http;

using Newtonsoft.Json;


namespace CrestronNhlCloud.Transport
{
	public class HttpTransport : IDisposable
	{
		private readonly HttpClient _httpClient;

		public HttpTransport()
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

		public string SetBaseUrl { private get; set; } = "http://statsapi.web.nhl.com/api/v1/";

		public List<NhlApiShared.Api.Models.Teams.Team> GetTeams()
		{
			var httpClientRequest = new HttpClientRequest();
			httpClientRequest.Url.Parse($"{SetBaseUrl}teams");
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
			httpClientRequest.Url.Parse($"{SetBaseUrl}schedule?teamId={teamId}");
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