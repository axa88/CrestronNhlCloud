using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.DoorLocks;
using CrestronNhlCloud.PyngApi.Models.Response;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class DoorlockApi
	{
		public const string ResourceName = "doorlocks";

		public static DoorLocksResponse GetAll() => RestClient.RequestSync<DoorLocksResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static DoorLocksResponse GetOne(int id) => RestClient.RequestSync<DoorLocksResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));

		public static PostResponseBinary SetState(int id, DoorLockMode mode) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}/{mode}"));
	}
}
