using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Response;
using CrestronNhlCloud.PyngApi.Models.Scenes;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class SceneApi
	{
		public const string ResourceName = "scenes";

		public static ScenesResponse GetAll() => RestClient.RequestSync<ScenesResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static ScenesResponse GetOne(int id) => RestClient.RequestSync<ScenesResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));

		public static PostResponseBinary SetState(int id) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/recall/{id}"));
	}
}
