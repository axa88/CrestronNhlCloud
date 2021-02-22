using System;


namespace Crestron.RAD.Common.Logging
{
	/// <summary>
	/// Used for more verbose logging by the framework.
	/// </summary>
	public class Logger
	{
		/// <summary>
		/// Specifies if logging is enabled on the system
		/// </summary>
		public bool LoggingEnabled;

		/// <summary>
		/// The current logging level.
		/// By default, Error will be selected.
		/// </summary>
		public LoggingLevel CurrentLevel;

		private Action<string> _driverLogger;

		public Logger(Action<string> driverLogger)
		{
			_driverLogger = driverLogger;
		}

		public void Debug(string message)
		{
			if (CurrentLevel == LoggingLevel.Debug)
				_driverLogger?.Invoke($"[DEBUG] - {message}");
		}

		public void Warning(string message)
		{
			if (CurrentLevel >= LoggingLevel.Warning)
				_driverLogger?.Invoke($"[WARNING] - {message}");
		}

		public void Error(string message)
		{
			if (CurrentLevel >= LoggingLevel.Error)
				_driverLogger?.Invoke($"[ERROR] - {message}");
		}
	}
}