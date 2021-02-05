using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public enum FanMode
	{
		Auto,
		Off, // undocumented, found in mis-configured BACNet device. If adding fan attribute but not configured it will select off by default
		On,
		Low,
		Medium,
		High,
		Circulate,
		InternalError // bug 155626
	}

	public class ThermostatFanMode
	{
		public ThermostatFanMode(int id, FanMode fanMode)
		{
			Id = id;
			FanMode = fanMode;
		}

		[JsonProperty(PropertyName = "id")]
		public int Id;

		[JsonProperty(PropertyName = "fanMode")]
		public FanMode FanMode;
	}
}
