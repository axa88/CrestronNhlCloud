using Crestron.RAD.Common.Enums;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.DeviceTypes.ExtensionDevice
{
	internal static class ExtensionDevicePropertyHelper
	{
		internal static bool VerifyValueType<T>(DevicePropertyType type)
		{
			switch (type)
			{
				case DevicePropertyType.Boolean:
					if (typeof(T) == typeof(bool))
						return true;
					break;
				case DevicePropertyType.LocalizedString:
				case DevicePropertyType.String:
					if (typeof(T) == typeof(string))
						return true;
					break;
				case DevicePropertyType.Int16:
					if (typeof(T) == typeof(short))
						return true;
					break;
				case DevicePropertyType.UInt16:
					if (typeof(T) == typeof(ushort))
						return true;
					break;
				case DevicePropertyType.Int32:
					if (typeof(T) == typeof(int))
						return true;
					break;
				case DevicePropertyType.UInt32:
					if (typeof(T) == typeof(uint))
						return true;
					break;
				case DevicePropertyType.Int64:
					if (typeof(T) == typeof(long))
						return true;
					break;
				case DevicePropertyType.UInt64:
					if (typeof(T) == typeof(ulong))
						return true;
					break;
				case DevicePropertyType.Float:
					if (typeof(T) == typeof(float))
						return true;
					break;
				case DevicePropertyType.Double:
					if (typeof(T) == typeof(double))
						return true;
					break;
			}

			return false;
		}
	}
}