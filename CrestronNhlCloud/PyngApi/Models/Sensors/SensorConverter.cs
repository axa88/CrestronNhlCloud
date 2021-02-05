using System;

using CrestronNhlCloud.PyngApi.Serialization;

using Newtonsoft.Json.Linq;


namespace CrestronNhlCloud.PyngApi.Models.Sensors
{
	public class SensorConverter : CustomConverter<Sensor>
	{
		protected override Sensor Create(Type objectType, JObject jObject)
		{
			//if (jObject.ContainsKey("level")) return jObject.ToObject<SensorVariableType>(); // requires c#8
			if (jObject["level"] != null) return jObject.ToObject<SensorVariableType>();
			//if (jObject.ContainsKey("presence")) return jObject.ToObject<SensorBinaryType>(); // requires c#8
			if (jObject["presence"] != null) return jObject.ToObject<SensorBinaryType>();
			//if (jObject.ContainsKey("door sensor") || jObject.ContainsKey("battery level")) return jObject.ToObject<SensorDoorType>(); // requires c#8
			if (jObject["door sensor"] != null || jObject["battery level"] != null) return jObject.ToObject<SensorDoorType>();
			return new Sensor();
		}
	}
}
