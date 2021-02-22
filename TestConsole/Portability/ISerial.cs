using System;


namespace Crestron.RAD.Common.Interfaces
{
    public interface ISerial : IDisposable
    {
        /// <summary>
        /// Disconnects the connection
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Reestablishes the connection
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Establishes the connection
        /// </summary>
        void Connect();
    }
}
