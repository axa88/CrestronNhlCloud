namespace Crestron.RAD.Common.Interfaces
{
    public interface ITransportLogger : IBasicLogger
    {
        bool LogTxAndRxAsBytes { set; }
    }
}
