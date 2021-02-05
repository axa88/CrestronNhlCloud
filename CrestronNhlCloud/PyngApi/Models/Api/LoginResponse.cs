using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Api
{
	public class LoginResponse : Error
	{
		[JsonProperty(PropertyName = "authKey")]
		public string AuthKey { get; set; }
	}
}
