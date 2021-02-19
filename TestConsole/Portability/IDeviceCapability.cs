namespace Crestron.DeviceDrivers.API
{
    /// <summary>
    /// Interface that Device Capability interfaces should implement.
    /// There is no functionality here, it's just used to tag interfaces which
    /// are Device Capabilities.
    /// </summary>
    public interface IDeviceCapability
    {
    }

    /// <summary>
    /// Describes an device which has capabilites that can be queried.
    /// </summary>
    public interface IDeviceWithCapabilities
    {
        /// <summary>
        /// Queries for a capability of the device
        /// </summary>
        /// <typeparam name="T">The capability to retrieve</typeparam>
        /// <returns>An object implementing the capability, or null if the capability is not supported.</returns>
        T TryGetCapability<T>() where T : class, IDeviceCapability;
    }
}
