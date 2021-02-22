using System;
using Crestron.RAD.Common.Interfaces;


namespace Crestron.RAD.Common.Transports
{
    public interface ISerialTransport : ITransport, ITransportLogger
    {
        bool IsConnected { get;}
        int DriverID { get; set; }

        Action<string> DataHandler { set; }
        Action<string, object[]> Send { get; set; }

        void Start();
        void Stop();

        Action<string> MessageTimedOut { set; }
        uint TimeOut { get; set; }
        Action<bool> ConnectionChanged { set; }
    }
}
