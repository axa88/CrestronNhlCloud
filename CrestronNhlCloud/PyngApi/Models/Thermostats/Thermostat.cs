using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public enum ThermostatType
	{
		//[EnumMember(Value = "thermostat")]
		Thermostat
	}

	public enum SystemMode
	{
		Off,
		Auto,
		AuxHeat,
		Cool,
		Heat
	}

	public enum TemperatureUnits
	{
		FahrenheitWholeDegrees,
		CelsiusWholeDegrees,
		CelsiusHalfDegrees
	}


	public class Thermostat : BaseDevice
	{
		[JsonProperty(PropertyName = "currentMode")]
		public SystemMode CurrentMode { get; set; }

		[JsonProperty(PropertyName = "currentSetPoint")]
		public List<SetPoint> CurrentSetPoint { get; set; }

		[JsonProperty(PropertyName = "CurrentTemperature")]
		public ushort CurrentTemperature { get; set; }

		[JsonProperty(PropertyName = "temperatureUnits")]
		public TemperatureUnits TemperatureUnits { get; set; }

		[JsonProperty(PropertyName = "currentFanMode")]
		public FanMode CurrentFanMode { get; set; }

		[JsonProperty(PropertyName = "schedulerState")]
		public SchedulerState SchedulerState { get; set; }

		[JsonProperty(PropertyName = "availableFanModes")]
		public List<FanMode> AvailableFanModes { get; set; }

		[JsonProperty(PropertyName = "availableSystemModes")]
		public List<SystemMode> AvailableSystemModes { get; set; }

		[JsonProperty(PropertyName = "availableSetPoints")]
		public List<AvailableSetPoint> AvailableSetPoints { get; set; }
	}
}