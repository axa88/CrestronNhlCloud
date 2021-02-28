// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common
{
	/// <summary>
	/// Information about an Extension Device driver.
	/// </summary>
	public class ExtensionDeviceData
	{
		/// <summary>
		/// True if this driver implements the IExtensionDevice interface
		/// </summary>
		public bool IsExtensionDevice { get; set; }

		/// <summary>
		/// True if this device is a media device (ie. Custom A/V Switcher)
		/// </summary>
		public bool IsMediaDevice { get; set; }

		/// <summary>
		/// True is this device is a media source (ie. Custom Media Server, Custom NVR)
		/// </summary>
		public bool IsMediaSource { get; set; }
	}
}