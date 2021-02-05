using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.Api;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class LoginApi
	{
		public const string ResourceName = "login";

		public static LoginResponse Login() => RestClient.RequestSync<LoginResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthToken, $"{ResourceName}"));
	}
}
