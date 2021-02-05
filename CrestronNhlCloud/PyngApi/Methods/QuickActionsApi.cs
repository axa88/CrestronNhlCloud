using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.ApiClients;
using CrestronNhlCloud.PyngApi.Models.QuickActions;
using CrestronNhlCloud.PyngApi.Models.Response;


// ReSharper disable MemberCanBePrivate.Global
namespace CrestronNhlCloud.PyngApi.Methods
{
	public static class QuickActionsApi
	{
		public const string ResourceName = "quickactions";

		public static QuickActionResponse GetAll() => RestClient.RequestSync<QuickActionResponse>(RestClient.CreateRequest(RequestType.Get, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}"));

		public static PostResponseBinary SetQuickAction(int id, QuickActionCommand command) => RestClient.RequestSync<PostResponseBinary>(RestClient.CreateRequest(RequestType.Post, RestClient.HeaderType.Crestron_RestAPI_AuthKey, $"{ResourceName}/{id}/{command}"));
	}
}
