using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public class ThermostatMode
	{
		public ThermostatMode(int id, SystemMode systemMode)
		{
			Id = id;
			SystemMode = systemMode;
		}

		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "mode")]
		public SystemMode SystemMode { get; set; }
	}
}