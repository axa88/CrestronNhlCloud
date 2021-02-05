using System;

using CrestronNhlCloud.PyngApi.Serialization;

using Newtonsoft.Json.Linq;


namespace CrestronNhlCloud.PyngApi.Models.Scenes
{
	public class SceneConverter : CustomConverter<Scene>
	{
		// requires c#8
		//protected override Scene Create(Type objectType, JObject jObject) => jObject.ContainsKey("subType") ? jObject.ToObject<Scene>() : new Scene(jObject.ToObject<SceneV2>());
		protected override Scene Create(Type objectType, JObject jObject) => jObject["subType"] != null ? jObject.ToObject<Scene>() : new Scene(jObject.ToObject<SceneV2>());
	}
}
