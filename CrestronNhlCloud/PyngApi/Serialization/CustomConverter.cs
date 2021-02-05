using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CrestronNhlCloud.PyngApi.Serialization
{
	/// <inheritdoc />
	/// <summary>Base Generic JSON Converter that can help quickly define converters for specific types by automatically
	/// generating the CanConvert, ReadJson, and WriteJson methods, requiring the implementer only to define a strongly typed Create method.</summary>
	public abstract class CustomConverter<T> : JsonConverter
	{
		/// <summary>Create an instance of objectType, based properties in the JSON object</summary>
		/// <param name="objectType">type of object expected</param>
		/// <param name="jObject">contents of JSON object that will be deserialized</param>
		protected abstract T Create(Type objectType, JObject jObject);

		/// <inheritdoc />
		/// <summary>Determines if this converted is designed to deserialization to objects of the specified type.</summary>
		/// <param name="objectType">The target type for deserialization.</param>
		/// <returns>True if the type is supported.</returns>
		public override bool CanConvert(Type objectType) => typeof(T).IsAssignableFrom(objectType);

		/// <inheritdoc />
		/// <summary>Parses the json to the specified type.</summary>
		/// <param name="reader">Newtonsoft.Json.JsonReader</param>
		/// <param name="objectType">Target type.</param>
		/// <param name="existingValue">Ignored</param>
		/// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
		/// <returns>Deserialized Object</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			// Load JObject from stream
			var jObject = JObject.Load(reader);

			// Create target object based on JObject
			var target = Create(objectType, jObject);

			//Create a new reader for this jObject, and set all properties to match the original reader.
			var jObjectReader = jObject.CreateReader();
			jObjectReader.Culture = reader.Culture;
			// requires Json.Net > 4 < 11, not needed anyway
			/*jObjectReader.DateParseHandling = reader.DateParseHandling;
			jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
			jObjectReader.FloatParseHandling = reader.FloatParseHandling;*/

			// Populate the object properties
			serializer.Populate(jObjectReader, target);

			return target;
		}

		/// <inheritdoc />
		/// <summary>Serializes to the specified type</summary>
		/// <param name="writer">Newtonsoft.Json.JsonWriter</param>
		/// <param name="value">Object to serialize.</param>
		/// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);
	}
}