using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.Methods;
using CrestronNhlCloud.PyngApi.Models.Credentials;
using CrestronNhlCloud.PyngApi.Models.Errors;
using CrestronNhlCloud.Transport;

using static System.Environment;

using RequestType = Crestron.SimplSharp.Net.Http.RequestType;


namespace CrestronNhlCloud.PyngApi.ApiClients
{
	public static class RestClient
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		internal enum HeaderType
		{
			None,
			Crestron_RestAPI_AuthToken,
			Crestron_RestAPI_AuthKey,
		}

		private static readonly HttpClient HttpClient = new HttpClient { UserAgent = "WearHome", Timeout = 8000 };

		private static string _key = string.Empty;
		private static string _token = string.Empty;
		private static readonly Regex ValidIpOptionalPort = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(:[0-9]+)?$");
		private static readonly Regex ValidHostname = new Regex(@"^(?=.{1,255}$)[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?(?:\.[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?)*\.?$");
		//private static readonly Regex ValidToken = new Regex(@"^[a-zA-Z0-9]{12}");
		private static readonly Regex ValidToken = new Regex(@"^[a-zA-Z0-9]");

		//static HttpSSClient() => RestClient.AddHandler("application/json", () => CustomNewtonsoftJsonSerializer.Instance);
		static RestClient()
		{
			HttpClient.Accept = "application/json";
		}
		public static bool IsValidName(string name) => !string.IsNullOrWhiteSpace(name);
		public static bool IsValidIpOptionalPort(string ipOptionalPort) => ValidIpOptionalPort.IsMatch(ipOptionalPort);
		public static bool IsValidHostname(string hostName) => ValidHostname.IsMatch(hostName);
		public static bool IsValidToken(string token) => ValidToken.IsMatch(token) && (token.Length == 1 || token.Length == 12);
		public static bool IsValidPort(string port) => ushort.TryParse(port, out var p) && p != default;

		public static Error UpdateSession(Credentials credentials, CancellationToken cancellationToken)
		{
			CrestronConsole.PrintLine("update");

			var (isResolved, ipOrException) = ResolveHost(credentials.Host);
			var usingHostName = !ValidIpOptionalPort.IsMatch(credentials.Host);

			if (!isResolved)
				return new Error { Success = false, ErrorMessage = $"Could not {(usingHostName ? "resolve Host" : "find Address")}:{NewLine}{credentials.Host}{NewLine}{ipOrException}" };

			CrestronConsole.PrintLine("resolved");

			_token = credentials.Token;
			HttpClient.Url.Parse($@"http://{ipOrException}/cws/api");

			// check for security, v2.0.2 or later
			var api = ApiApi.GetApi();
			if (api.Success)
			{
				CrestronConsole.PrintLine(api.Version);

				var login = LoginApi.Login();
				if (login.Success)
				{
					_key = login.AuthKey;
					login.ErrorMessage = usingHostName ? $"{credentials.Host} @ {ipOrException}" : ipOrException;
				}
				else
					_key = "";

				return login;
			}
			else
				CrestronConsole.PrintLine("else what");

			CrestronConsole.PrintLine(api.ErrorMessage);
			CrestronConsole.PrintLine("ret");

			return api;
		}

		internal static T RequestSync<T>(HttpClientRequest restRequest) where T : Error, new()
		{
			try
			{
				var response = ResponseHandlers.Response<T>(HttpClient.Dispatch(restRequest));
				if (response.HttpStatusCode == 511)
				{
					var login = LoginApi.Login();
					if (!login.Success)
						return new T { Success = false, ErrorMessage = login.ErrorMessage, HttpStatusCode = login.HttpStatusCode };

					_key = login.AuthKey;

					var headers = new HttpHeaders();
					restRequest.Header = headers;
					restRequest.Header.AddHeader(new HttpHeader("Crestron-RestAPI-AuthKey", _key));

					return ResponseHandlers.Response<T>(HttpClient.Dispatch(restRequest));
				}

				return response;
			}
			catch (Exception e)
			{
				return new T { Success = false, ErrorMessage = e.Message };
			}
		}

		internal static HttpClientRequest CreateRequest(RequestType method, HeaderType headerType, string resource, int? timeout = null)
		{
			// var restRequest = new HttpClientRequest(method) { RequestFormat = DataFormat.Json, Resource = resource, JsonSerializer = new JsonSerializer() };
			var restRequest = new HttpClientRequest { RequestType = method };

			//if (timeout != null) restRequest.ThisClient.Timeout = timeout.Value; // set in client??
			//restRequest.AddHeader("Accept", "application/json");
			//restRequest.Header.AddHeader(new HttpHeader("Accept", "application/json")); already set in client

			if (headerType != HeaderType.None)
			{
				var headerKey = headerType.ToString().Replace('_', '-');
				var headerValue = headerType == HeaderType.Crestron_RestAPI_AuthKey ? _key : _token;
				restRequest.Header.AddHeader(new HttpHeader(headerKey, headerValue));
			}

			return restRequest;
		}

		private static (bool isResolved, string ipOrException) ResolveHost(string host)
		{
			try { return /*ValidIpOptionalPort.IsMatch(host) ? (true, host) :*/ (true, Dns.GetHostEntry(host).AddressList.First().ToString()); }
			catch (Exception e) { return (false, e.Message); }
		}

		private static Task<T> WithWaitCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
		{
			return task.IsCompleted ? task : task.ContinueWith(completedTask =>
										completedTask.ConfigureAwait(false).GetAwaiter().GetResult(),
										cancellationToken,
										TaskContinuationOptions.ExecuteSynchronously,
										TaskScheduler.Default);
		}
	}
}
