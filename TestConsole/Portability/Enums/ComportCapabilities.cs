// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

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