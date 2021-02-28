using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Crestron.RAD.Common.Enums;
using System.Text;
using System.Threading;

using RADCommon.Interfaces;
using Crestron.DeviceDrivers.API;

using CrestronQueue = System.Collections.Queue;
using CTimer = System.Threading.Timer;


namespace Crestron.RAD.Common.ExtensionMethods
{
	public static class ExtensionMethods
	{
		internal static bool TryDispose(this CrestronQueue queue)
		{
			var disposed = false;
			if (queue.Exists())
			{
				queue?.Clear();
				disposed = true;
			}
			return disposed;
		}

		public static void AppendToStringBuffer(this object appendObject, System.Text.StringBuilder builder, object lockObject)
		{
			try
			{
				lock (lockObject)
					builder.Append(appendObject);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: Crestron.RAD.Common.ExtensionMethods.AppendToStringBuffer - {0}", e);
			}
		}

		public static string ToString(this StringBuilder builder, object lockObject)
		{
			var result = string.Empty;
			try
			{
				lock (lockObject)
					result = builder.ToString();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: Crestron.RAD.Common.ExtensionMethods.ToString - {0}", e);
			}

			return result;
		}

		public static void Clear(this StringBuilder builder, object lockObject)
		{
			try
			{
				lock (lockObject)
					builder.Length = 0;

				// Setting the capacity is a problem on Mono systems.  Over time it
				// takes longer and longer.  It is not necessary anyway since it
				// will free the memory buffer and force it to be re-allocated.
				// builder.Capacity = 0;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: Crestron.RAD.Common.ExtensionMethods.Clear - {0}", e.ToString());
			}
		}


		public static string PadCharacter(this string inputString, Parameters parameter)
		{
			try
			{
				var padChar = Convert.ToChar(parameter.PadCharacter.Unescape());
				switch (parameter.PadDirection)
				{
					case Parameters.PadDirections.Left:
						return inputString.PadLeft(parameter.StaticDataWidth, padChar);
					case Parameters.PadDirections.Right:
						return inputString.PadRight(parameter.StaticDataWidth, padChar);
					default:
						return inputString;
				}
			}
			catch (ArgumentNullException ex)
			{
				return inputString;
			}
		}

		/// <summary>
		/// Check that a string is not null or whitespace
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static bool HasValue(this string inputString)
		{
			return !inputString.IsNullOrWhiteSpace();
		}

		/// <summary>
		/// Is Null or White/Empty space check for string
		/// </summary>
		/// <param name="inputstring"></param>
		/// <returns></returns>
		public static bool IsNullOrWhiteSpace(this string inputstring)
		{
			return ((inputstring.DoesNotExist())
				|| (string.Empty == inputstring.Trim()));
		}

		/// <summary>
		/// Is Null check
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <returns></returns>
		public static bool DoesNotExist<T>(this T field)
		{
			return (null == field);
		}

		/// <summary>
		/// Is not null check
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field"></param>
		/// <returns></returns>
		public static bool Exists<T>(this T field)
		{
			return (null != field);
		}

		/// <summary>
		/// Get safe command string for device
		/// Replace double backslashes with single
		/// </summary>
		/// <param name="commandString"></param>
		/// <param name="exception">The exception thrown by calling Regex.Unescape(commandString)</param>
		/// <returns></returns>
		internal static string GetSafeCommandString(this string commandString, out Exception exception)
		{
			exception = null;
			if (!commandString.IsNullOrWhiteSpace() && 
				commandString.Contains("\\u"))
			{
				try
				{
					commandString = Regex.Unescape(commandString);
				}
				catch (Exception e)
				{
					exception = e;
				}
			}
			return commandString;
		}

		/// <summary>
		/// Get safe command string for device
		/// Replace double backslashes with single
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public static string GetSafeCommandString(this string commandString)
		{
			Exception exception = null;
			return GetSafeCommandString(commandString, out exception);
		}

		/// <summary>
		/// Replace double backslashes with single
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public static string Unescape(this string inputString)
		{
			return inputString.HasValue() ?
				Regex.Unescape(inputString) :
				inputString;
		}

		/// <summary>
		/// Replace single backslashes with double
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public static string Escape(this string inputString)
		{
			return inputString.HasValue() ?
				Regex.Escape(inputString) :
				inputString;
		}

		/// <summary>
		/// Unescape each value in given dictionary.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dictionary"></param>
		public static void UnescapeDictionaryValues<T>(this Dictionary<T, string> dictionary)
		{
			if (dictionary.Exists())
			{
				foreach (var item in dictionary.ToArray())
				{
					dictionary[item.Key] = item.Value.Unescape();
				}
			}
		}

		/// <summary>
		/// If disposableObj exists then dispose it.
		/// And if it is a timer stop it before dispose.
		/// </summary>
		/// <param name="disposableObj"></param>
		/// <returns>true/false - whether dispose has been called.</returns>
		public static bool TryDispose(this IDisposable disposableObj)
		{
			bool disposed;
			if (disposableObj.Exists())
			{
				if (disposableObj is CTimer timer)
				{
					timer.Change(Timeout.Infinite, Timeout.Infinite);
					timer.Dispose();
					disposed = true;
				}
				else
				{
					disposableObj.Dispose();
					disposed = true;
				}
			}
			else
			{
				disposed = false;
			}

			return disposed;
		}


		/// <summary>
		/// Truncates a string to the specified max length
		/// </summary>
		internal static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}

		internal static string EnumToString(this UserAttributeType value)
		{
			switch (value)
			{
				case UserAttributeType.DeviceId:
					return "DeviceId";
				case UserAttributeType.OnScreenId:
					return "OnScreenId";
				case UserAttributeType.MessageBox:
					return "MessageBox";
				case UserAttributeType.Custom:
					return "Custom";
				case UserAttributeType.Url:
					return "Url";
				default:
					return "";
			}
		}

		internal static string EnumToString(this UserAttributeRequiredForConnectionType value)
		{
			switch (value)
			{
				case UserAttributeRequiredForConnectionType.None:
					return "None";
				case UserAttributeRequiredForConnectionType.Before:
					return "Before";
				case UserAttributeRequiredForConnectionType.After:
					return "After";
				default:
					return "";
			}
		}

		internal static string EnumToString(this UserAttributeDataType value)
		{
			switch (value)
			{
				case UserAttributeDataType.String:
					return "String";
				case UserAttributeDataType.Number:
					return "Number";
				case UserAttributeDataType.Boolean:
					return "Boolean";
				case UserAttributeDataType.Hex:
					return "Hex";
				default:
					return "";
			}
		}

		/// <summary>
		/// Extension method to expose a generic API to register an interface on an IInterfaceLookup.
		/// Note that this is going to use typeof(T) to get the interface, not implementation.GetType()
		/// to avoid hard-to-predict results. This does mean, though, that if you call this with
		/// an instance of "object" it's not going to register anything since typeof(object) isn't going
		/// to have any interfaces that implement TBaseInterface.
		/// Use this API when you have an object that's already typed as the type you want to register.
		/// </summary>
		/// <typeparam name="T">The type to register</typeparam>
		/// <typeparam name="TBaseInterface">The base interface type of the lookup (usually ICapability)</typeparam>
		/// <param name="lookup">The lookup object</param>
		/// <param name="implementation">The object implementing the interface T to register</param>
		public static void RegisterInterface<T, TBaseInterface>(this IInterfaceLookup<TBaseInterface> lookup, T implementation)
			where T : class, TBaseInterface
			where TBaseInterface : class
		{
			lookup.RegisterInterface(typeof(T), implementation);
		}

		/// <summary>
		/// Extension method to get an interface from an IInterfaceLookup and cast it to the
		/// desired type. This method works for any type of IInterfaceLookup but requires
		/// specifying the TBaseInterface for the syntax to work.
		/// </summary>
		/// <typeparam name="T">The interface to look up</typeparam>
		/// <typeparam name="TBaseInterface">The base interface of the IInterfaceLookup</typeparam>
		/// <param name="lookup">The IInterfaceLookup object</param>
		/// <returns>An instance of T, or null if not found</returns>
		public static T TryGetInterface<T, TBaseInterface>(this IInterfaceLookup<TBaseInterface> lookup)
			where T : class, TBaseInterface
			where TBaseInterface : class
		{
			return (T)lookup.TryGetInterface(typeof(T));
		}

		/// <summary>
		/// Extension method to get an interface from an IInterfaceLookup of ICapability
		/// and cast it to the desired type.
		/// </summary>
		/// <typeparam name="T">The interface to look up</typeparam>
		/// <param name="lookup">The IInterfaceLookup object where TBaseInterface is ICapabiity</param>
		/// <returns>An instance of T, or null if not found</returns>
		public static T TryGetInterface<T>(this IInterfaceLookup<IDeviceCapability> lookup)
			where T : class, IDeviceCapability
		{
			return (T)lookup.TryGetInterface(typeof(T));
		}
	}
}
