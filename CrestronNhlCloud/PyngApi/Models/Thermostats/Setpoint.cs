using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public class SetPoint : SetPointBase
	{
		[JsonConstructor]
		public SetPoint(SetPointType type, ushort temperature)
		{
			Temperature = temperature;
			Type = type;
		}

		public SetPoint(SetPointType type, int temperature)
		{
			Temperature = temperature;
			Type = type;
		}

		[JsonProperty(PropertyName = "temperature")]
		public int Temperature;
	}
}
