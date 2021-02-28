namespace Crestron.RAD.Common.Enums
{
    /// <summary>
    /// Hardware Handshake type for the com port
    ///
    /// </summary>
    public enum eComHardwareHandshakeType
    {
        NotSpecified = -1,
        ComspecHardwareHandshakeNone = 0,
        ComspecHardwareHandshakeRTS = 1,
        ComspecHardwareHandshakeCTS = 2,
        ComspecHardwareHandshakeRTSCTS = 3,
    }
}