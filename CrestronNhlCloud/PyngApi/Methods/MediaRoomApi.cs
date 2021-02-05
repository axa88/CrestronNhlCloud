using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Media;
using CrestronNhlCloud.PyngApi.Models.Response;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class MediaRoomApi
	{
		public const string ResourceName = "mediarooms";

		public static MediaRoomsResponse GetAll() => RestClient.RequestSync<MediaRoomsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static MediaRoomsResponse GetOne(int id) => RestClient.RequestSync<MediaRoomsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));

		public static PostResponseBinary SetMute(int id, MuteCommand state) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}/{state}"));

		public static PostResponseBinary SetVolume(int id, int level) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}/{VolumeDiscreteCommand.Volume}/{level}"));

		public static PostResponseBinary SetSource(int id, int sourceId) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}/{SourceCommand.SelectSource}/{sourceId}"));

		public static PostResponseBinary SetPower(int id, PowerState state) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}/{PowerCommand.Power}/{state}"));
	}
}
