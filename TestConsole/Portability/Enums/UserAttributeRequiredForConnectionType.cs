// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

using Crestron.RAD.Common.Interfaces;

namespace Crestron.RAD.Common.Enums
{
    public enum UserAttributeRequiredForConnectionType
    {
		/// <summary>
		/// Specifies that the user attribute is not required in order to connect to the device.
		/// </summary>
        None = 1,

		/// <summary>
		/// Specifes that the user attribute is required before connecting to the device.
		/// These user attributes must be set before calling <see cref="IConnection.Connect"/>.
		/// </summary>
        Before = 2,

		/// <summary>
		/// Specifies that the user attribute is required after connecting to the device.
		/// </summary>
        After = 3
    }
}