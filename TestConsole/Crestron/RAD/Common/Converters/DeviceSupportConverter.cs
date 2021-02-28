using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Crestron.RAD.Common.Enums;


namespace Crestron.RAD.Common
{
    /// <summary>
    /// Handles the DeviceSupport node of the driver's embeddeded JSON-formatted file.
    /// This will attempt to parse the key as <see cref="CommonFeatureSupport"/>.
    /// </summary>
    public class DeviceSupportConverter : JsonConverter
    {
        /// <summary>
        /// Overrides to check if the passed parameter is of type Dictionary<CommonFeatureSupport, bool>.
        /// </summary>
        /// <param name="objectType">Type being converted</param>
        /// <returns>True if this is the DeviceSupport dictionary.</returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Dictionary<CommonFeatureSupport, bool>));
        }

        /// <summary>
        /// Handles parsing the dictionary key within a try/catch to allow a driver to function
        /// when invalid <see cref="CommonFeatureSupport"/> values are specified.
        /// </summary>
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var supportedFeatures = new Dictionary<CommonFeatureSupport, bool>();
            foreach (var supportPair in jo)
            {
                try
                {
                    var key = (CommonFeatureSupport)Enum.Parse(typeof(CommonFeatureSupport), supportPair.Key, true);
                    var value = supportPair.Value.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase);

                    supportedFeatures[key] = value;
                }
                catch
                {
                    Console.WriteLine($"Notice: Invalid CommonFeatureSupport found in JSON. Skipping {supportPair.Key}");
                }
            }
            return supportedFeatures;
        }

        /// <summary>
        /// Not implemented, drivers do not generate JSON.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
