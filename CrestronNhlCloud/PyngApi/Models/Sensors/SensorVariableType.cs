using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Sensors
{
	public class SensorVariableType : Sensor
	{
		[JsonProperty(PropertyName = "level")]
		public int Level;
	}
}
