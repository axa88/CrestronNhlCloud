using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Devices;
using CrestronNhlCloud.PyngApi.Models.Scenes;
using CrestronNhlCloud.PyngApi.Models.Sensors;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace CrestronNhlCloud.PyngApi.Serialization
{
	public static class SerializerSettings
	{
		public static readonly JsonSerializerSettings Jss = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Error,
				NullValueHandling = NullValueHandling.Include,
				Converters = new List<JsonConverter> { new StringEnumConverter() }
			};

		public static readonly JsonSerializerSettings Jss4Devices = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Error,
				NullValueHandling = NullValueHandling.Include,
				Converters = new List<JsonConverter> { new StringEnumConverter(), new DeviceConverter() }
			};

		public static readonly JsonSerializerSettings Jss4Sensors = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Error,
				NullValueHandling = NullValueHandling.Include,
				Converters = new List<JsonConverter> { new StringEnumConverter(), new SensorConverter() }
			};

		public static readonly JsonSerializerSettings Jss4Scenes = new JsonSerializerSettings
			{
			   MissingMemberHandling = MissingMemberHandling.Error,
			   NullValueHandling = NullValueHandling.Include,
			   Converters = new List<JsonConverter> { new StringEnumConverter(), new SceneConverter() }
			};

		public static readonly JsonSerializer Js = new JsonSerializer
			{
				Converters = { new StringEnumConverter() }
			};
	}
}
