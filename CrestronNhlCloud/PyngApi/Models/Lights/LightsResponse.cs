using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Lights
{
	public class LightsResponse : Error
	{
		[JsonProperty(PropertyName = "lights")]
		public List<Light> Lights { get; set; }
	}
}
