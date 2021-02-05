using System.Collections.Generic;
using System.ComponentModel;

using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.SecurityDevices
{
	public enum SecurityDeviceType
	{
		[Description("security Device")]
		//[EnumMember(Value = "security Device")]
		SecurityDevice
	}

	public enum AvailableState
	{
		Alarm,
		ArmAway,
		ArmInstant,
		ArmStay,
		Disarmed,
		EntryDelay,
		ExitDelay,
		Fire,
		Unknown
	}

	public enum CurrentState
	{
		Alarm,
		ArmAway,
		ArmInstant,
		ArmStay,
		Disarmed,
		EntryDelay,
		ExitDelay,
		Fire,
		Unknown
	}


	public class SecurityDevice : BaseDevice
	{
		[JsonProperty(PropertyName = "availableStates")]
		public List<AvailableState> AvailableStates;

		[JsonProperty(PropertyName = "currentState")]
		public CurrentState CurrentState;
	}
}
