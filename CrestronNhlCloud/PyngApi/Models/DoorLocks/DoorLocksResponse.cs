using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.DoorLocks
{
	public class DoorLocksResponse : Error
	{
		[JsonProperty(PropertyName = "doorLocks")]
		public List<DoorLock> DoorLocks { get; set; }
	}
}
