using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Sensors;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class SensorApi
	{
		public const string ResourceName = "sensors";

		public static SensorsResponse GetAll() => RestClient.RequestSync<SensorsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static SensorsResponse GetOne(int id) => RestClient.RequestSync<SensorsResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));
	}
}
