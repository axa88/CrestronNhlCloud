using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Api;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class LogoutApi
	{
		public const string ResourceName = "logout";

		public static LogoutResponse Logout() => RestClient.RequestSync<LogoutResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));
	}
}
