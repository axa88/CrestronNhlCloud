using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Shades
{
	public class ShadesResponse : Error
	{
		[JsonProperty(PropertyName = "shades")]
		public List<Shade> Shades { get; set; }
	}
}
