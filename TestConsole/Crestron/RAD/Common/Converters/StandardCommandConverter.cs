using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Crestron.RAD.Common.Enums;


namespace Crestron.RAD.Common
{
	public class StandardCommandConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(Dictionary<StandardCommandsEnum, Commands>));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jo = JObject.Load(reader);
			var standardCommands = new Dictionary<StandardCommandsEnum, Commands>();
			foreach (var commandPair in jo)
			{
				try
				{
					var key = (StandardCommandsEnum)Enum.Parse(typeof(StandardCommandsEnum), commandPair.Key, true);
					var value = JsonConvert.DeserializeObject<Commands>(commandPair.Value.ToString());
					standardCommands[key] = value;
				}
				catch
				{
					Console.WriteLine($"Notice: Invalid StandardCommandsEnum found in JSON. Skipping {commandPair.Key}");
				}
			}
			return standardCommands;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
