using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public class AvailableSetPoint : SetPointBase
	{
		[JsonProperty(PropertyName = "minValue")]
		public ushort MinValue;

		[JsonProperty(PropertyName = "maxValue")]
		public ushort MaxValue;
	}
}
