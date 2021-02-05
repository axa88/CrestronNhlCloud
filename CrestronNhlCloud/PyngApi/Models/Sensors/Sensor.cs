using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Sensors
{
	public enum SensorType
	{
		//[EnumMember(Value = "sensor")]
		Sensor
	}

	public enum SensorSubType
	{
		BurglaryAlarm,
		Door,
		Doorbell,
		DoorSensor,
		DrivewaySensor,
		FireAlarm,
		OccupancySensor,
		PhotoSensor,
		SmokeAlarm,
		Window,
		WaterAlarm
	}

	public class Sensor : BaseDevice
	{
		[JsonProperty(PropertyName = "subType")]
		public SensorSubType SubType;
	}
}
