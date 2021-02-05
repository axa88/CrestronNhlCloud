using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Api
{
	public class LogoutResponse : Error
	{
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }
	}
}
