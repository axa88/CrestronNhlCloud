using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Rooms
{
	public class RoomsResponse : Error
	{
		[JsonProperty(PropertyName = "rooms")]
		public List<Room> Rooms { get; set; }
	}
}
