using System.Collections.Generic;

using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Response;
using CrestronNhlCloud.PyngApi.Models.Thermostats;
using CrestronNhlCloud.PyngApi.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class ThermostatApi
	{
		public const string ResourceName = "thermostats";

		public static ThermostatsResponse GetAll() => RestClient.RequestSync<ThermostatsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static ThermostatsResponse GetOne(int id) => RestClient.RequestSync<ThermostatsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));

		public static PostResponseBinary SetPoint(int id, IEnumerable<SetPoint> setPoints)
		{
			const string resource = "setPoint";
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{resource}");
			restRequest.ContentString = new JObject(new JProperty("id", id), new JProperty($"{resource}s", JToken.FromObject(setPoints, JsonSerializer.Create(SerializerSettings.Jss)))).ToString();
			return RestClient.RequestSync<PostResponseBinary>(restRequest);
		}

		public static PostResponseTrinary SetMode(IEnumerable<ThermostatMode> modes)
		{
			const string resource = "mode";
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{resource}");
			restRequest.ContentString = new JObject(new JProperty(ResourceName, JToken.FromObject(modes, JsonSerializer.Create(SerializerSettings.Jss)))).ToString();
			return RestClient.RequestSync<PostResponseTrinary>(restRequest);
		}

		public static PostResponseTrinary SetFan(IEnumerable<ThermostatFanMode> modes)
		{
			const string resource = "fanmode";
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{resource}");
			restRequest.ContentString = new JObject(new JProperty(ResourceName, JToken.FromObject(modes, JsonSerializer.Create(SerializerSettings.Jss)))).ToString();
			return RestClient.RequestSync<PostResponseTrinary>(restRequest);
		}

		public static PostResponseTrinary SetSchedule(IEnumerable<Schedule> modes)
		{
			const string resource = "Schedule";
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{resource}");
			restRequest.ContentString = new JObject(new JProperty(ResourceName, JToken.FromObject(modes, JsonSerializer.Create(SerializerSettings.Jss)))).ToString();
			return RestClient.RequestSync<PostResponseTrinary>(restRequest);
		}
	}
}
