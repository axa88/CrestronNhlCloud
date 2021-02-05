using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Sensors
{
	public class SensorsResponse : Error
	{
		[JsonProperty(PropertyName = "sensors")]
		public List<Sensor> Sensors { get; set; }
	}
}
