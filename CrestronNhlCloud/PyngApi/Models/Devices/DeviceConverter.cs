using System;
using System.Linq;

using CrestronNhlCloud.PyngApi.Models.DoorLocks;
using CrestronNhlCloud.PyngApi.Models.Lights;
using CrestronNhlCloud.PyngApi.Models.Response;
using CrestronNhlCloud.PyngApi.Models.SecurityDevices;
using CrestronNhlCloud.PyngApi.Models.Sensors;
using CrestronNhlCloud.PyngApi.Models.Shades;
using CrestronNhlCloud.PyngApi.Models.Thermostats;
using CrestronNhlCloud.PyngApi.Serialization;

using Newtonsoft.Json.Linq;


namespace CrestronNhlCloud.PyngApi.Models.Devices
{
	public class DeviceConverter : CustomConverter<BaseDevice>
	{
		protected override BaseDevice Create(Type objectType, JObject jObject)
		{
			//if (jObject.ContainsKey("type")) // requires Json.Net 11
			if (jObject["type"] != null)
			{
				var name = ((string)jObject["type"])?.Replace(" ", "");
				if (Enum.TryParse(name, true, out DeviceType _))
				{
					var deviceTypes = Enum.GetNames(typeof(DeviceType));
					var matchingType = deviceTypes.Where(x => string.Equals(x, name, StringComparison.CurrentCultureIgnoreCase)).ToArray();
					if (matchingType.Any())
					{
						if (matchingType.First().Equals(DeviceType.Drape.ToString(), StringComparison.CurrentCultureIgnoreCase)
							|| matchingType.First().Equals(DeviceType.Shade.ToString(), StringComparison.CurrentCultureIgnoreCase)
							|| matchingType.First().Equals(DeviceType.ShadeGroup.ToString(), StringComparison.CurrentCultureIgnoreCase))
							return jObject.ToObject<Shade>(SerializerSettings.Js);
						if (matchingType.First().Equals(DeviceType.Lock.ToString(), StringComparison.CurrentCultureIgnoreCase))
							return jObject.ToObject<DoorLock>();
						if (matchingType.First().Equals(DeviceType.SecurityDevice.ToString(), StringComparison.CurrentCultureIgnoreCase))
							return jObject.ToObject<SecurityDevice>();
						if (matchingType.First().Equals(DeviceType.Thermostat.ToString(), StringComparison.CurrentCultureIgnoreCase))
							return jObject.ToObject<Thermostat>();
						if (matchingType.First().Equals(DeviceType.Sensor.ToString(), StringComparison.CurrentCultureIgnoreCase))
							return jObject.ToObject<Sensor>();
					}
				}

				throw new MissingMemberException("Device : type");
			}

			//if (jObject.ContainsKey("subType") && Enum.TryParse((string)jObject["subType"]!, out LightSubType _)) // requires Json.Net 11 & c#8
			if (jObject["subType"] != null && Enum.TryParse((string)jObject["subType"], out LightSubType _))
				return jObject.ToObject<Light>();

			throw new ArgumentNullException(jObject.ToString(), ToString());
		}
	}
}
