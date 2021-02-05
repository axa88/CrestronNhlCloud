using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public class Base
	{
		[JsonProperty(PropertyName = "id")]
		public ushort Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
	}
}
