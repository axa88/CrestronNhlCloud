namespace Crestron.RAD.Common.Enums
{
    /// <summary>
    /// Fields in .json that if specified will generate fields that are adjustable in resulting com .dat file
    /// </summary>
    public enum eTransportAdjustableProperties
    {
        ComspecAdjustableBaud = 0,
        ComspecAdjustableParity = 1,
        ComspecAdjustableDataBits = 2,
        ComspecAdjustableStopBits = 3,
        ComspecAdjustableHardwareHandshaking = 4,
        ComspecAdjustableSoftwareHandshaking = 5,
        ComspecAdjustableDeviceId = 6,
        EthernetAdjustablePort = 7,
        EthernetAdjustableDeviceId = 8
    }
}