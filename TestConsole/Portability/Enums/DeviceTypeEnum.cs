// Copyright (C) 2017 to the present, Crestron Electronics, Inc.
// All rights reserved.
// No part of this software may be reproduced in any form, machine
// or natural, without the express written consent of Crestron Electronics.
// Use of this source code is subject to the terms of the Crestron Software License Agreement 
// under which you licensed this source code.

using System;

namespace Crestron.RAD.Common.Enums
{
    public enum DeviceTypes
    {
        FlatPanelDisplay = 1,
        BlurayPlayer = 2,
        CableBox = 3,
        VideoServer = 4,
        VideoConferenceCodec = 5,
        AlarmSystem = 6,
        PtzCamera = 7,
        AvReceiver = 8,
        Projector = 9,
        SecuritySystem = 10,
        Unknown = 11,
        PoolController = 12,

		/// <summary>
		/// Audio amplifier
		/// </summary>
		Amplifier = 13,

		/// <summary>
		/// Stove, oven, refridgerator, coffee maker, microwave, dishwasher
		/// </summary>
		Appliance = 14,

		/// <summary>
		/// Audio processor, pre-amp
		/// </summary>
		AudioProcessor = 15,

		/// <summary>
		/// Ceiling fan, vent fan, stove hood
		/// </summary>
		Fan = 16,

		/// <summary>
		/// Gas/electric fireplace
		/// </summary>
		Fireplace = 17,

		/// <summary>
		/// Xbox, Playstation, Nintendo
		/// </summary>
		GameConsole = 18,

		/// <summary>
		/// Garage door opener
		/// </summary>
		GarageDoor = 19,

		/// <summary>
		/// Thermostat, heating, air conditioner, humidifier, dehumidifier
		/// </summary>
		Hvac = 20,

		/// <summary>
		/// Lawn sprinkler, garden irrigation
		/// </summary>
		IrrigationSystem = 21,

		/// <summary>
		/// Light switch, smart bulb, LED controller
		/// </summary>
		Light = 22,

		/// <summary>
		/// Door lock, pad lock, etc.
		/// </summary>
		Lock = 23,

		/// <summary>
		/// Smart outlet, smart plug
		/// </summary>
		Outlet = 24,

		/// <summary>
		/// Ups, power conditioner
		/// </summary>
		PowerController = 25,

		/// <summary>
		/// Document printer
		/// </summary>
		Printer = 26,

		/// <summary>
		/// Motorized projector lift
		/// </summary>
		ProjectorLift = 27,

		/// <summary>
		/// Wifi router
		/// </summary>
		Router = 28,

		/// <summary>
		/// Document scanner
		/// </summary>
		Scanner = 29,

		/// <summary>
		/// Motorized projector screen
		/// </summary>
		Screen = 30,

		/// <summary>
		/// Door sensor, occupancy sensor, light sensor
		/// </summary>
		Sensor = 31,

		/// <summary>
		/// Smart speaker
		/// </summary>
		Speaker = 32,

		/// <summary>
		/// Window shades, curtains, drapes
		/// </summary>
		Shade = 33,

		/// <summary>
		/// Robot vacuum, pool vacuum
		/// </summary>
		Vacuum = 34,

		/// <summary>
		/// Tesla, wifi/cell connected car
		/// </summary>
		Vehicle = 35,


        /// <summary>
        /// Obsolete, please use Platform instead
        /// Gateway device contains list of paired devices.
        /// </summary>
		[Obsolete]
        Gateway = 36,

        /// <summary>
        /// Obsolete, please use AVSwitcher instead
        /// Audio/Video Switcher
        /// </summary>
        [Obsolete]
        AudioVideoSwitcher = 37,

		/// <summary>
		/// Device type which contains a list of paired devices.
		/// </summary>
		Platform = 38,
        
        /// <summary>
        /// Audio/Video Switcher
        /// </summary>
        AVSwitcher = 39
    }
}
