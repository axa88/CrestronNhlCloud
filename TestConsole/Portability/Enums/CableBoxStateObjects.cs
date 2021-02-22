// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

namespace Crestron.RAD.Common.Enums
{
    public enum CableBoxStateObjects
    {
        Power = 1, 
        Channel = 2, 
        Audio = 3, 
        Connection = 4, 
        EnergyStar = 5,
        Authentication = 6,
        Volume = 7,
        Mute = 8,
        WarmingUp = 9,
        WarmedUp = 10,
        CoolingDown = 11,
        CooledDown = 12,
        PoweredOn = 13,
        PoweredOff = 14,
        ActiveMediaService = 15,
        ActiveMediaServicePlaybackState = 16,
        WarmupTimeChanged = 30,
        CooldownTimeChanged = 31
    }
}