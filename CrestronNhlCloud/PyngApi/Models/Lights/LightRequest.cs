using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Lights
{
	public class LightRequest
	{
		public LightRequest(int id, string level, string time)
		{
			Id = id;
			Level = level;
			Time = time;
		}

		public LightRequest(Base light, int level, int time)
		{
			Id = light.Id;
			Level = level.ToString();
			Time = time.ToString();
		}

		[JsonProperty(PropertyName = "id")]
		public int Id;

		// request and response use different types
		[JsonProperty(PropertyName = "level")]
		public string Level { get; set; }

		[JsonProperty(PropertyName = "time")]
		public string Time { get; set; }
	}
}
