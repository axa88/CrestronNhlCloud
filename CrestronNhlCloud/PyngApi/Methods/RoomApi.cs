using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Rooms;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class RoomApi
	{
		public const string ResourceName = "rooms";

		public static RoomsResponse GetAll() => RestClient.RequestSync<RoomsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static RoomsResponse GetOne(int id) => RestClient.RequestSync<RoomsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));
	}
}
