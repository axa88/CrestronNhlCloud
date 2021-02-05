using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.SecurityDevices;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class SecurityApi
	{
		public const string ResourceName = "securityDevices";

		public static SecurityDevicesResponse GetAll() => RestClient.RequestSync<SecurityDevicesResponse>(RestClient.CreateRequest( RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static SecurityDevicesResponse GetOne(int id) => RestClient.RequestSync<SecurityDevicesResponse>(RestClient.CreateRequest( RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}"));
	}
}
