using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;
using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Devices
{
	public class DevicesResponse : Error
	{
		[JsonProperty(PropertyName = "devices")]
		public List<BaseDevice> Devices { get; set; }
	}
}
