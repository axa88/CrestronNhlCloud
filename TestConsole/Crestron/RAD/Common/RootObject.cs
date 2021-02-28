using Crestron.RAD.Common.Enums;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Crestron.RAD.Common.BasicDriver
{
	public class BaseRootObject
	{
		public CrestronSerialDeviceApi CrestronSerialDeviceApi { get; set; }
	}

	public class CrestronSerialDeviceApi
	{
		public GeneralInformation GeneralInformation { get; set; }

		public Api Api { get; set; }

		public Dictionary<CommonFeatureSupport, bool> DeviceSupport;

		public List<string> DeviceSupport2;

		public List<UserAttribute> UserAttributes { get; set; }

		/// <summary>
		/// A list of dll file names that this class depends on. These dlls must be included in the pkg.
		/// </summary>
		public List<string> Dependencies { get; set; }
	}

	public class Api
	{
		public Communication Communication { get; set; }
		public Dictionary<StandardCommandsEnum, Commands> StandardCommands { get; set; }
		public List<CustomCommand> CustomCommands { get; set; }
		public PowerWaitPeriod PowerWaitPeriod { get; set; }

		/// <summary>
		/// Indicates if the driver is able to detect configuration changes on the device.
		/// <para>
		/// This will be null if the feature is not applicable to a device.
		/// This will be true if the driver is able to detect configuration changes and will report the changes
		/// This will be false if the driver not able to detect configuration changes, and <see cref="Reconnect"/>
		/// must be called to get the new configuration.
		/// </para>
		/// </summary>
		public bool? SupportsConfigurationUpdate { get; set; }

		/// <summary>
		/// See <see cref="Feedback"/>.
		/// </summary>
		public Feedback Feedback { get; set; }

		/// <summary>
		/// Do not reference this in driver.
		/// This is for device-type libraries only.
		/// </summary>
		public MediaServiceProviderData MediaServiceProviders { get; set; }
	}

	/// <summary>
	/// Represents basic feedback information applicable to all drivers.
	/// </summary>
	public class Feedback
	{
		private CommandAckNak _commandAckNak;

		/// <summary>
		/// See <see cref="CommandAckNak"/>.
		/// </summary>
		public CommandAckNak CommandAckNak
		{
			get
			{
				if (_commandAckNak == null)
					_commandAckNak = new CommandAckNak();

				return _commandAckNak;
			}
			set
			{
				_commandAckNak = value;
			}
		}

		/// <summary>
		/// The header that is common to all responses received from the device.
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// The minimum amount of time in ms an application would need to wait before expecting
		/// the driver to report true feedback after an operation.
		/// </summary>
		public uint MinimumResponseTime { get; set; }

		/// <summary>
		/// The maximum amount of time in ms an application would need to wait before expecting
		/// the driver to report true feedback after an operation.
		/// </summary>
		public uint MaximumResponseTime { get; set; }

		/// <summary>
		/// Lets a driver define if the API supports providing updates without polling.
		/// If this is enabled, then the driver's polling sequence will only be sent until each
		/// individual poll command receives a response. The driver will poll again like this
		/// if the connection state goes from false to true.
		/// </summary>
		public bool SupportsUnsolicitedFeedback { get; set; }

		/// <summary>
		/// The command a driver should send using the polling interval to illicit a response
		/// from the device. This can be any command that causes no operation to be performed on the device
		/// such as a power poll. This must be used if the driver does not use an ethernet transport to keep
		/// track of disconnects.
		/// <para>This is only used when <see cref="SupportsUnsolicitedFeedback"/> is set to true.</para>
		/// </summary>
		public string ConnectedStatePollCommand { get; set; }
	}

	/// <summary>
	/// Represents scaling information for features such as volume or bass levels.
	/// </summary>
	public class ScalingInformation
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScalingInformation()
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="minLevel"><see cref="MinimumDeviceLevel"/></param>
		/// <param name="maxLevel"><see cref="MaximumDeviceLevel"/></param>
		/// <param name="stepSize"><see cref="DeviceStepSize"/></param>
		public ScalingInformation(float minLevel, float maxLevel, float stepSize)
		{
			MaximumDeviceLevel = maxLevel;
			MinimumDeviceLevel = minLevel;
			DeviceStepSize = stepSize;
		}

		/// <summary>
		/// Specifies the maximum level the device will report.
		/// This is used for internal scaling logic.
		/// </summary>
		[JsonProperty]
		public float MaximumDeviceLevel { get; private set; }

		/// <summary>
		/// Specifies the minimum level the device will report.
		/// This is used for internal scaling logic.
		/// </summary>
		[JsonProperty]
		public float MinimumDeviceLevel { get; private set; }

		/// <summary>
		/// Specifies the step size of one volume step in either direction.
		/// This is used for internal scaling logic.
		/// </summary>
		[JsonProperty]
		public float DeviceStepSize { get; private set; }
	}
}