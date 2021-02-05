using System;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public abstract class PostResponseBase : Error
	{
		[JsonProperty(PropertyName = "status")]
		public Enum Status { get; set; }
	}
}
