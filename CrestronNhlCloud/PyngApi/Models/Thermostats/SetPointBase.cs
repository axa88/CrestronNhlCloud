using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public enum SetPointType
	{
		Off,
		Heat,
		Cool,
		Auto,
		Humidity,
		AuxHeat,
		Slab
	}

	public class SetPointBase
	{
		[JsonProperty(PropertyName = "type")]
		public SetPointType Type;
	}
}
