using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Lights
{
	public enum LightType
	{
		//[EnumMember(Value = "light")]
		Light,
	}

	public enum LightSubType
	{
		Dimmer,
		Switch,
		Fan
	}

	public enum FanSpeed
	{
		Off = 0,
		Low = 2,
		Medium = 3,
		High = 4
	}

	public class Light : BaseDevice
	{
		[JsonProperty(PropertyName = "subType")]
		public LightSubType SubType { get; set; }

		[JsonProperty(PropertyName = "level")]
		public ushort Level { get; set; }

		[JsonProperty(PropertyName = "speed")]
		public ushort Speed { get; set; }
	}
}
