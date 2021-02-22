// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

namespace Crestron.RAD.Common.Enums
{
    public enum SecuritySystemAreaType
    {
        CanArmDisarm = 0,
        CanArmDisarmTempBlock = 1,
        SingleDependantArea = 2,
        MultiDependantArea = 3,
        CanArmDisarmWithTimer = 4,
        ControlledByTimer = 5,
        BankVault = 6
    }
}