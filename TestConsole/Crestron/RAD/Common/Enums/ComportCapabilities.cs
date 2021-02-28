using System;


namespace Crestron.RAD.Common.Enums
{
    [Flags]
    public enum eComportCapabilities
    {
        COMPORT_SUPPORTS_RS232 = 1,
        COMPORT_SUPPORTS_RS422 = 2,
        COMPORT_SUPPORTS_RS485 = 4,
        COMPORT_SUPPORTS_RTS = 8,
        COMPORT_SUPPORTS_CTS = 16,
    }
}