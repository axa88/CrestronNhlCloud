using System.Collections.Generic;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public class PostResponseTrinary : PostResponseBase
 {
		[JsonProperty(PropertyName = "status")]
		public new StatusTrinary Status { get; set; }

		[JsonProperty(PropertyName = "errorDevices")]
		public List<ushort> ErrorDevices { get; set; }
	}
}
