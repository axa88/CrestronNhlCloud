using System;
using Crestron.DeviceDrivers.API;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Interfaces
{
	/// <summary>
	/// Interface for drivers to implement if they require being initialized with persisted settings or additional files
	/// that were provided in the driver's pkg.
	/// </summary>
	public interface IDriverInitialization : IDeviceCapability
	{
		/// <summary>       
		/// Allows the driver to initialize any local and external resources along
		/// with any custom settings.
		/// </summary>
		/// <param name="driverPath">Path to the folder containing all of the files from the pkg.</param>
		/// <param name="driverSettings">
		/// A serialized custom object containing settings specific to the driver.
		/// This object is created by the driver and stored by the consuming application on <see cref="DriverSettingsChanged"/> event.
		/// </param>
		/// <returns>True if initialization succeeded, otherwise false.</returns>
		bool Initialize(string driverPath, string driverSettings);

		/// <summary>
		/// Serialized custom object containing settings that are specific to this driver.
		/// </summary>
		string DriverSettings { get; }

		/// <summary>
		/// Event indicating that the DriverSettings property has changed.
		/// The consuming application should store these settings and pass it back
		/// to the driver when calling the Initialize method.
		/// </summary>
		event EventHandler<DriverSettingsEventArgs> DriverSettingsChanged;
	}

	/// <summary>
	/// Event argument that contains the driver's settings.
	/// </summary>
	public class DriverSettingsEventArgs : EventArgs
	{
		/// <summary>
		/// Contructor for <see cref="DriverSettingsEventArgs"/>.
		/// </summary>
		/// <param name="driverSettings"></param>
		public DriverSettingsEventArgs(string driverSettings)
		{
			DriverSettings = driverSettings;
		}

		/// <summary>
		/// The object containing all of the driver's settings that it needs saved.
		/// </summary>
		public string DriverSettings { get; private set; }
	}
}