using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public class BaseResponse : Base
	{
		[JsonProperty(PropertyName = "roomId")]
		public ushort RoomId;

		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }
	}
}
