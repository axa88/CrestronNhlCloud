// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

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