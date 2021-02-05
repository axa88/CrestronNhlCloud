using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Lights
{
	public class FanRequest
	{
		public FanRequest(int id, int speed)
		{
			Id = id;
			Speed = speed;
		}

		public FanRequest(Base light, int speed)
		{
			Id = light.Id;
			Speed = speed;
		}

		[JsonProperty(PropertyName = "id")]
		public int Id;

		// request and response use different types
		[JsonProperty(PropertyName = "speed")]
		public int Speed { get; set; }

		//[JsonProperty(PropertyName = "connectionStatus")]
		//public ConnectionStatus ConnectionStatus { get; set; }

		//public bool ShouldSerializeConnectionStatus() { return false; }
	}
}
