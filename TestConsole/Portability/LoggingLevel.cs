using System;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
//using Crestron.SimplSharp.CrestronDataStore;

namespace Crestron.RAD.Common.Logging
{
    /// <summary>
    ///  Error - Exceptions
    ///  Warning - Exceptions and warnings
    ///  Debug - Exceptions, warnings, and everything else
    ///  
    /// Logs will only print if EnableLogging is enabled.
    /// Drivers should not reference this enumeration.
    /// Drivers should only check the property EnableLogging before making a call to the method Log.
    /// </summary>
    public enum LoggingLevel
    {
        Error = 1,
        Warning = 2,
        Debug = 3
    }
}