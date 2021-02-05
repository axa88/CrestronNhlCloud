using Crestron.SimplSharp.Net.Http;

using CrestronNhlCloud.PyngApi.Models.Devices;
using CrestronNhlCloud.PyngApi.Models.Errors;
using CrestronNhlCloud.PyngApi.Models.Scenes;
using CrestronNhlCloud.PyngApi.Models.Sensors;
using CrestronNhlCloud.PyngApi.Serialization;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.ApiClients
{
	internal static class ResponseHandlers
	{
		// RestSharp or possibly Mono has chronic problem in handling non 200 status http requests.
		// https://github.com/restsharp/RestSharp/issues/1280
		// https: //github.com/mono/mono/issues/9511
		// deep rooted exception throws away ALL response data, ver this versions 106.6.7 - 106.6.9 at least the sync call retains status code/status description.

		private const string LogMark = "!!!!";

		// sync/async version
		internal static T Response<T>(HttpClientResponse response) where T : Error, new() => Result<T>(response);

		private static T Result<T>(HttpClientResponse response) where T : Error, new()
		{ // some exceptions have status code so use it again request to figure out proper response
			if (response.Code == 200)
			{
				// check if this ever gets fixed and returns content
				#if DEBUG
				if (!string.IsNullOrWhiteSpace(response.ContentString))
					return new T { Success = false, ErrorMessage = "Response Exceptions now have content!!!", HttpStatusCode = response.Code };
				#endif

				try
				{
					var result = DeserializeObject<T>(response);
					result.HttpStatusCode = response.Code;
					result.Success = true;
					return result;
				}
				catch { return new T { Success = false, ErrorMessage = "response unreadable", HttpStatusCode = response.Code }; }
			}

			return new T { Success = false, ErrorMessage = "no work", HttpStatusCode = response.Code };
		}

		private static T DeserializeObject<T>(HttpClientResponse response)
		{
			JsonSerializerSettings jss;
			var type = typeof(T);
			if (type == typeof(DevicesResponse))
				jss = SerializerSettings.Jss4Devices;
			else if (type == typeof(SensorsResponse))
				jss = SerializerSettings.Jss4Sensors;
			else if (type == typeof(ScenesResponse))
				jss = SerializerSettings.Jss4Scenes;
			else
				jss = SerializerSettings.Jss;

			return JsonConvert.DeserializeObject<T>(response.ContentString, jss);
		}
	}
}
