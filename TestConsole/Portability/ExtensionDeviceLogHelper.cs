using System;
//using Crestron.SimplSharp;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.DeviceTypes.ExtensionDevice
{
	internal static class ExtensionDeviceLogHelper
	{
		/// <summary>
		/// Used to log a message from a private method where we may not want to show the name of the method
		/// because it doesn't really do the driver developer much good.
		/// </summary>
		internal static void LogMessage(Action<string> logMethod, LogLevel logLevel, string message)
		{
			// Log only warnings and errors to SimplSharp log so that we dont flood the err log with debug messages
			switch (logLevel)
			{
				case LogLevel.Debug:
					message = "[DEBUG] " + message;
					break;
				case LogLevel.Warning:
					//ErrorLog.Warn(message);
					Console.WriteLine($"Warn: {message}");
					message = "[WARNING] " + message;
					break;
				case LogLevel.Error:
					//ErrorLog.Error(message);
					Console.WriteLine($"Error: {message}");
					message = "[ERROR] " + message;
					break;
			}

			// Always log to CCD log
			logMethod(message);
		}

		/// <summary>
		/// Used to log a message with the class and method name included.
		/// This should only be used by public and protected methods that the driver developer
		/// can see so that they know where the error is.
		/// </summary>
		internal static void LogMessage(
			Action<string> logMethod,
			LogLevel logLevel,
			string className,
			string methodName,
			string message)
		{
			LogMessage(logMethod, logLevel, string.Format("{0}.{1} - {2}", className, methodName, message));
		}
	}

	internal enum LogLevel
	{
		Debug,
		Warning,
		Error
	}
}