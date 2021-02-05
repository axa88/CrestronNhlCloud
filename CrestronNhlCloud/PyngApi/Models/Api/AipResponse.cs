using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Api
{
	public class ApiResponse : Error
	{
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
	}
}
