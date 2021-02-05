using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public enum ConnectionStatus
	{
		//[EnumMember(Value = "offline")]
		Offline,

		//[EnumMember(Value = "online")]
		Online
	}

	public class BaseDevice : BaseResponse
	{
		[JsonProperty(PropertyName = "connectionStatus")]
		public ConnectionStatus ConnectionStatus { get; set; }
	}
}
