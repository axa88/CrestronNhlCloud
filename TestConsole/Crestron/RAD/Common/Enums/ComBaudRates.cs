using System;

namespace Crestron.RAD.Common.Enums
{
    /// <summary>
    /// Enumeration of the com port Baud Rates
    /// </summary>
    [Flags]
    public enum eComBaudRates
    {
        NotSpecified = -1,
        ComspecBaudRate300 = 1,
        ComspecBaudRate600 = 2,
        ComspecBaudRate1200 = 4,
        ComspecBaudRate1800 = 8,
        ComspecBaudRate2400 = 16,
        ComspecBaudRate3600 = 32,
        ComspecBaudRate4800 = 64,
        ComspecBaudRate7200 = 128,
        ComspecBaudRate9600 = 256,
        ComspecBaudRate14400 = 512,
        ComspecBaudRate19200 = 1024,
        ComspecBaudRate28800 = 2048,
        ComspecBaudRate38400 = 4096,
        ComspecBaudRate57600 = 8192,
        ComspecBaudRate115200 = 65536,
    }
}