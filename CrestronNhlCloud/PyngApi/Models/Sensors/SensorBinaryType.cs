using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Sensors
{
	public enum SensorPresence
	{
		Unavailable,
		Occupied,
		Vacant,
		OpenOrOn,
		CloseOrOff
	}

	public class SensorBinaryType : Sensor
	{
		[JsonProperty(PropertyName = "presence")]
		public SensorPresence Presence;
	}
}
