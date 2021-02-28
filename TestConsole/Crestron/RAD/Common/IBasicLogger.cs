using System;
using Crestron.DeviceDrivers.API;


namespace Crestron.RAD.Common.Interfaces
{
    public interface IBasicLogger : IDeviceCapability
    {
        /// <summary>
        /// Property to enable TX Debug statements.
        /// </summary>
        bool EnableTxDebug { get;  set; }

        /// <summary>
        /// Property to enable RX Debug statements.
        /// </summary>
        bool EnableRxDebug { get;  set; }

        /// <summary>
        /// Property to enable Logging statements.
        /// </summary>
        bool EnableLogging { get; set; }

        /// <summary>
        /// Property set a Custom Logger.
        /// </summary>
        Action<string> CustomLogger { get; set; }

    }
}
