using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Api;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class ApiApi
	{
		public const string ResourceName = "api";

		public static ApiResponse GetApi() => RestClient.RequestSync<ApiResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.None, $"{ResourceName}"));
	}
}
