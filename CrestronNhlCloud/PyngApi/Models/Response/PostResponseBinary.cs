using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public class PostResponseBinary : PostResponseBase
	{
		[JsonProperty(PropertyName = "status")]
		public new StatusBinary Status { get; set; }
	}
}
