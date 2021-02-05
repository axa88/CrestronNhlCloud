using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Media
{
	public class MediaRoomsResponse : Error
	{
		[JsonProperty(PropertyName = "mediaRooms")]
		public List<MediaRoom> MediaRooms { get; set; }
	}
}
