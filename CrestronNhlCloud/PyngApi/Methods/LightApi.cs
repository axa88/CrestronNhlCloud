using System.Collections.Generic;

using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Lights;
using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json.Linq;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class LightApi
	{
		public const string ResourceName = "lights";

		public static LightsResponse GetAll() => RestClient.RequestSync<LightsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static LightsResponse GetOne(int id) => RestClient.RequestSync<LightsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));

		public static PostResponseTrinary SetState(IEnumerable<LightRequest> lights)
		{
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/SetState");
			restRequest.ContentString = new JObject(new JProperty(ResourceName, JToken.FromObject(lights))).ToString();
			return RestClient.RequestSync<PostResponseTrinary>(restRequest);
		}

		public static PostResponseTrinary SetState(IEnumerable<FanRequest> fans)
		{
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/SetState");
			restRequest.ContentString = new JObject(new JProperty(ResourceName, JToken.FromObject(fans))).ToString();
			return RestClient.RequestSync<PostResponseTrinary>(restRequest);
		}
	}
}
