// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

namespace Crestron.RAD.Common.Enums
{
    public enum BlurayPlayerStateObjects
    {
        Power = 1, 
        PlayBackStatus = 2, 
        MediaInfo = 3, 
        Connection = 4, 
        EnergyStar = 5,
        Authentication = 6,
        WarmingUp = 7,
        WarmedUp = 8,
        CoolingDown = 9,
        CooledDown = 10,
        PoweredOn = 11,
        PoweredOff = 12,
        WarmupTimeChanged = 30,
        CooldownTimeChanged = 31
    }
}