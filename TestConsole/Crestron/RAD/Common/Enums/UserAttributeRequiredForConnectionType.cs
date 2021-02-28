namespace Crestron.RAD.Common.Enums
{
	public enum UserAttributeRequiredForConnectionType
	{
		/// <summary>
		/// Specifies that the user attribute is not required in order to connect to the device.
		/// </summary>
		None = 1,

		/// <summary>
		/// Specifes that the user attribute is required before connecting to the device.
		/// These user attributes must be set before calling <see cref="IConnection.Connect"/>.
		/// </summary>
		Before = 2,

		/// <summary>
		/// Specifies that the user attribute is required after connecting to the device.
		/// </summary>
		After = 3
	}
}