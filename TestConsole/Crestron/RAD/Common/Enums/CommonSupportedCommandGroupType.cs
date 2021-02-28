﻿namespace Crestron.RAD.Common.Enums
{
	/// <summary>
	/// The group a <see cref="CommandSet"/> could be a part of.
	/// </summary>
	public enum CommonCommandGroupType
	{
		Volume = 1,
		Mute = 2,
		MicMute = 3,
		ConferenceCall = 4,
		DisplayName = 5,
		RemoteNumber = 6,
		CallbackNumber = 7,
		CallType = 8,
		CallSpeed = 9,
		CallDirection = 10,
		CallStatus = 11,
		Camera = 12,
		Input = 13,
		Selfview = 14,
		Dial = 15,
		Phonebook = 16,
		Video = 17,
		Booking = 18,
		Other = 19,
		SystemName = 20,
		SystemH323Id = 21,
		SystemE164Alias = 22,
		SystemSoftwareVersion = 23,
		SystemSerialNumber = 24,
		SystemInfo = 25,
		AllowFecc = 26,
		MuteMicsAutoAnswer = 27,
		DoNotDisturb = 28,
		AutoAnswer = 29,
		Standby = 30,
		Encryption = 31,
		Notification = 32,
		NotificationButtonOneText = 33,
		NotificationButtonTwoText = 34,
		NotificationButtonThreeText = 35,
		NotificationButtonFourText = 36,
		NotificationButtonFiveText = 37,
		NotificationType = 38,
		NotificationText = 39,
		NotificationTitle = 40,
		Unknown = 41,
		Presentation = 42,
		SystemIpAddress = 43,
		SystemSipUri = 44,
		NotificationClear = 45,
		NotificationDuration = 46,
		Multipoint = 47,
		MonitorPresentationSetting = 48,
		IrRemoteEmulation = 49,
		VideoMute = 50,
		Power = 51,
		LampHours = 52,
		EnergyStar = 53,
		OnScreenDisplay = 54,
		Arrow = 55,
		Keypad = 56,
		Channel = 57,
		Page = 58,
		PlayBackStatus = 59,
		TrackFeedback = 60,
		ChapterFeedback = 61,
		TrackElapsedTime = 62,
		TrackRemainingTime = 63,
		ChapterElapsedTime = 64,
		ChapterRemainingTime = 65,
		TotalElapsedTime = 66,
		TotalRemainingTime = 67,
		Connection = 68,
		AckNak = 69,
		Reboot = 70,
		Audio = 71,
		SurroundMode = 72,
		Tuner = 73,
		Output = 74,
		Bass = 75,
		Treble = 76,
		Loudness = 77,
		ToneControl = 78,
		ToneState = 79,
		TunerFrequency = 80,
		TunerFrequencyBand = 81,
		AudioInput = 82,
		Arm = 83,
		Disarm = 84,
		Bypass = 85,
		Unbypass = 86,
		Login = 87,
		SoftwareVersionDifferences = 88,
		MonitoringDeviceInfo = 89,
		//MonitoringResourceStatus = 90,
		MonitoringResourceName = 91,
		MonitoringResourcePermissionArea = 92,
		MonitoringResourcePermissionZone = 93,
		//MonitoringResourcePermissionOutput = 94,
		//MonitoringResourcePermissionDevice = 95,
		//MonitoringResourcePermissionDoor = 96,
		MonitoringLog = 97,
		Heartbeat = 98,
		MonitoringSystemStatus = 99,
		MonitoringSystemFeatures = 100,
		MonitoringSystemSetup = 101,
		MonitoringAreaInfo = 102,
		MonitoringZoneInfo  =  103,
		//MonitoringOutputInfo = 104,
		//MonitoringDoorInfo = 105,
		MonitoringAreaCount = 106,
		MonitoringZoneCount = 107,
		//MonitoringOutputCount = 108,
		//MonitoringDoorCount = 109,
		//MonitoringDeviceCount = 110,
		MonitoringAreaResourceStatus = 111,
		MonitoringZoneResourceStatus = 112,
		//MonitoringOutputResourceStatus = 113,
		//MonitoringDoorResourceStatus = 114,
		//MonitoringDeviceResourceStatus = 115,
		MonitoringAreaInExitDelay = 114,
		MonitoringAreaInEntryDelay = 115,
		MonitoringInfo = 116,
		SetResourceState = 117,
		MonitoringBypassedZones = 118,
		SilenceSensors = 119,
		ResetSensors = 120,
		//SubscribeResourceOnOffState = 121,
		SubscribeResourceReadyState = 122,
		//SubscribeResourceGeneralState = 123,
		SubscribeDeviceMessages = 124,
		SubscribeDeviceLogging = 125,
		SubscribeDeviceConfigurationChange = 126,
		Subscribe = 127,
		MonitoringAlarm = 128,
		MonitoringAreasNotReadyToArm = 129,
		ArmAll = 130,
		DisarmAll = 131,
		MediaService = 132,
		MediaServicePlaybackState = 133,
		MediaServiceSubscriptionState = 134,
		ArtworkMode = 135,
		DisplayMode = 136,
		MonitoringUsers = 137,
		MonitoringAreaAlarm = 138,
		MonitoringZoneAlarm = 139,
		MonitoringTamper = 140,
		MonitoringAlarmMemory = 141,
		MonitoringAreaAlarmMemory = 142,
		MonitoringZoneAlarmMemory = 143,
		MonitoringFireAlarm = 144,
		MonitoringFireAlarmMemory = 145,
		MonitoringAreaExitTime = 146,
		MonitoringSoftwareVersion = 147,
		AvrZone1 = 148,
		AvrZone2 = 149,
		AvrZone3 = 150,
		AvrZone4 = 151,
		AvrZone5 = 152,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IAudioVideoSwitcher"/> for all routing commands.
		/// </summary>
		AudioVideoSwitcher = 200,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IAudioVideoExtender"/> for all commands. Extenders utilize
		/// <see cref="Crestron.RAD.Common.BasicDriver.CommandSet.SubCommandGroup"/> to provide more information about the type of command
		/// being sent.
		/// </summary>
		AudioVideoExtender = 201,

		/// <summary>
		/// Used by commands made by <see cref="Crestron.RAD.Common.Interfaces.ISpeakerProtect"/>.
		/// </summary>
		SpeakerProtect = 202,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IHardwareDiagnostics"/>.
		/// </summary>
		Temperature = 203,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IHardwareDiagnostics"/>.
		/// </summary>
		TemperatureUnits = 204,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IAudioVideoExtender"/>.
		/// </summary>
		DcFaultState = 205,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IAudioVideoExtender"/>.
		/// </summary>
		ClippingAudio = 206,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.ICecPort"/>.
		/// </summary>
		CecPortCommunication = 207,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IDeviceComPort"/>.
		/// </summary>
		ComPortCommunication = 208,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IRelayPort"/>.
		/// </summary>
		RelayPortCommunication = 209,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IGenericIo"/>.
		/// </summary>
		GenericIoAnalogCommunication = 210,

		/// <summary>
		/// Used by <see cref="Crestron.RAD.Common.Interfaces.IGenericIo"/>.
		/// </summary>
		GenericIoDigitalCommunication = 211
	}
}