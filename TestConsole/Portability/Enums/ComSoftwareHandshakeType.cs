﻿// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

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