// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

namespace Crestron.RAD.Common.Enums
{
    public enum SecuritySystemStatus
    {
        Unknown = 0,
        Alarm = 1,
        ArmAway = 2,
        ArmInstant = 3,
        ArmStay = 4,
        Fire = 5,
        Disarmed = 6,
        Tamper = 7,
        EntryDelay = 8,
        ExitDelay = 9,
    }
}