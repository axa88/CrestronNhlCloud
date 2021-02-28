namespace Crestron.RAD.Common.Enums
{
    /// <summary>
    /// Software Handshake type for the com port
    ///
    /// </summary>
    public enum eComSoftwareHandshakeType
    {
        NotSpecified = -1,
        ComspecSoftwareHandshakeNone = 0,
        ComspecSoftwareHandshakeXON = 1,
        ComspecSoftwareHandshakeXONT = 2,
        ComspecSoftwareHandshakeXONR = 3,
    }
}