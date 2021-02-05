using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public class ThermostatsResponse : Error
	{
		[JsonProperty(PropertyName = "thermostats")]
		public List<Thermostat> Thermostats { get; set; }
	}
}
