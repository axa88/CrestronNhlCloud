using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Errors;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.QuickActions
{
	public class QuickActionResponse : Error
	{
		[JsonProperty(PropertyName = "quickActions")]
		public List<QuickAction> QuickActions { get; set; }
	}
}
