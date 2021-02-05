using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Sensors
{
	public enum SensorDoorStatus
	{
		Unknown,
		Opened,
		Closed
	}

	public enum SensorBatteryLevel
	{
		Unknown,
		Low,
		Normal
	}

	public class SensorDoorType : Sensor
	{
		[JsonProperty(PropertyName = "door status")]
		public SensorDoorStatus Status;

		[JsonProperty(PropertyName = "battery level")]
		public SensorBatteryLevel BatteryLevel;
	}
}
