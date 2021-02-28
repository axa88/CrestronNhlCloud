using Crestron.RAD.Common.Enums;


namespace Crestron.RAD.Common.Transports
{
    public interface ITransport
    {
        TransportType TransportType { get; }
    }
}
