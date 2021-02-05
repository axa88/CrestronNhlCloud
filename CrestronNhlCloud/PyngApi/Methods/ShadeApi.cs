using System.Collections.Generic;

using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Response;
using CrestronNhlCloud.PyngApi.Models.Shades;

using Newtonsoft.Json.Linq;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class ShadeApi
	{
		public const string ResourceName = "shades";

		public static ShadesResponse GetAll() => RestClient.RequestSync<ShadesResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static ShadesResponse GetOne(int id) => RestClient.RequestSync<ShadesResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));

		public static PostResponseTrinary SetState(IEnumerable<Shade> shades)
		{
			var restRequest = RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/SetState");
			restRequest.ContentString = new JObject(new JProperty(ResourceName, JToken.FromObject(shades))).ToString();
			return RestClient.RequestSync<PostResponseTrinary>(restRequest);
		}
	}
}
