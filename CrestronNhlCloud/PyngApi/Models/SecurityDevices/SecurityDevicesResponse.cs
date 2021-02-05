using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.SecurityDevices
{
	public class SecurityDevicesResponse : Error
	{
		[JsonProperty(PropertyName = "securityDevices")]
		public List<SecurityDevice> SecurityDevices { get; set; }
	}
}
