using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Scenes
{
	public class ScenesResponse : Error
	{
		[JsonProperty(PropertyName = "scenes")]
		public List<Scene> Scenes { get; set; }
	}
}
