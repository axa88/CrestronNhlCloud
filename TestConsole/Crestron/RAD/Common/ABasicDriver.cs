using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Crestron.DeviceDrivers.API;
using Crestron.RAD.Common.BasicDriver.FakeFeedback;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.ExtensionMethods;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Transports;

using RADCommon.Interfaces;


namespace Crestron.RAD.Common.BasicDriver
{
	/// <summary>
	/// The abstract class that represent all drivers.
	/// It provides a default implementation of the following component interfaces:
	/// <para />IBasicInformation, IConnection2, IBasicLogger, IBasicInformation2, IDisposable, ISupportedCommandsHelper, and IModelFileSupport.
	/// <para />Drivers should not override anything here and should instead override the protocol class unless specified otherwise.
	/// </summary>
	public abstract class ABasicDriver
	{
		/// <summary>
		/// A logger object that should not be referenced within drivers.
		/// This was introduced for more logging levels in the framework.
		/// </summary>
		private Logger Logger;

		private Crestron.RAD.Common.Enums.DeviceTypes _deviceType;
		private bool _connected;

		/// Expected parameters when a user uses the console command CCDLoggingLevel
		/// </summary>
		private static string _debugLoggingLevelParamater = "debug";
		private static string _warningLoggingLevelParamater = "warning";
		private static string _errorLoggingLevelParamater = "error";

		private bool _disposed = false;
		//private IAuthentication _authentication;
		//private IAuthentication2 _authentication2;

		private readonly Dictionary<string, UserAttribute> _userAttributes = new Dictionary<string, UserAttribute>();
		private Dictionary<string, string> _storedStringUserAttributes;
		private Dictionary<string, ushort> _storedUshortUserAttributes;
		private Dictionary<string, bool> _storedBooleanUserAttributes;

		//private List<SupportedCommand> _supportedCommands;

		/// <summary>
		/// <para />The common driver data between all device types.
		/// This will be set automatically by ConvertJsonFileToDriverData in each device-type library.
		/// <para />This should not be used or referenced - use the following instead:
		/// <para />Display - DisplayData
		/// <para />Cable Box - CableBoxData
		/// <para />Video Server - VideoServerData
		/// <para />Bluray Player - BlurayPlayerData
		/// <para />AV Receiver - AvrData
		/// </summary>
		protected internal BaseRootObject DriverData;

		/// <summary>
		/// Used internally for logging.
		/// Value is auto-assigned at run-time and should not be changed.
		/// </summary>
		protected int DriverID;

		private static int _driversLoaded = 0;
		private static bool _consoleCommandsLoaded;

		private delegate void DriverConsoleCommand(ConsoleCommandType command, string args);
		private DriverConsoleCommand InternalDriverConsoleCommandHandler;

		private static event DriverConsoleCommand DriverConsoleCommandEvent;

		/// Lookup for objects which implement capabilities if not this object.
		/// Capabilities which are implemented by this object directly do not need to be added to this lookup.
		/// </summary>
		protected IInterfaceLookup<IDeviceCapability> Capabilities { get; set; }

		/// <summary>
		/// The transport used for communication with the device which is responsible for transmitting and receiving messages.
		/// <para />This should be set within the driver's constructor to a class that inmplements ATransportDriver.
		///
		/// [[[example]]]
		/// </summary>
		protected ISerialTransport ConnectionTransport;

		/// <summary>
		/// Used internally to for enabling/disabling RX logging.
		/// </summary>
		protected bool InternalEnableLogging;

		protected Action<string> InternalCustomLogger;

		/// <summary>
		/// Constructor
		/// <para />This will do the following:
		/// <para />Load the driver JSON file (the first file found that ends with .JSON)
		/// <para />(S# Pro Only)Add console commands if they have not already been loaded
		/// <para />Set the driver ID used for logging
		/// </summary>
		public ABasicDriver()
		{
			Logger = new Logger(Log) { CurrentLevel = LoggingLevel.Error };

			//_supportedCommands = new List<SupportedCommand>();

			LoadDriverData();


			DriverID = ++_driversLoaded;
			InternalDriverConsoleCommandHandler = DriverConsoleCommandCallback;
			DriverConsoleCommandEvent += InternalDriverConsoleCommandHandler;
			Connected = false;

			_storedStringUserAttributes = new Dictionary<string, string>();
			_storedUshortUserAttributes = new Dictionary<string, ushort>();
			_storedBooleanUserAttributes = new Dictionary<string, bool>();
			Capabilities = new InterfaceLookup<IDeviceCapability>();
		}

		private enum ConsoleCommandType
		{
			RADInfo = 1,
			TxDebug = 2,
			RxDebug = 3,
			StackTrace = 4,
			General = 5,
			LoggingLevel = 6,
			DriverStates = 7,
			CloudRegistration = 8
		}

		/// <summary>
		/// Used by the framework to fake-feedback for device-types that use one StateChange event
		/// with an enum that specifies the change.
		/// This will be instantiated when the driver's embedded JSON is deseriaized if SupportsFeedback is true.
		/// It only fakes feedback at this level, and not the protocol level. It does this by preventing
		/// events via StateChange from executing if that event is being faked. The controller will stop blocking events
		/// once true feedback matches the faked state.
		/// </summary>
		protected FeedbackController FakeFeedbackController { get; private set; }

		/// <summary>
		/// Used internally by each device-type library.
		/// <para />This is the CType of the device-type class that implements this class
		/// </summary>
		public abstract Type AbstractClassType { get; }

		/// <summary>
		/// Used internally by each device-type library to parse the JSON driver data and to set the BaseRootObject.
		/// <para />Drivers should not override this and applications should not invoke it.
		/// </summary>
		/// <param name="jsonString">The serialized JSON object</param>
		public abstract void ConvertJsonFileToDriverData(string jsonString);

		/// <summary>
		/// Used internally by all device-type libraries when they are initialized.
		/// </summary>
		/// <param name="driverData">The BaseRootObject</param>
		public void Initialize(BaseRootObject driverData)
		{
			DriverData = driverData;

			ParseDeviceTypeFromJsonFile();

			if (DriverData.Exists() && DriverData.CrestronSerialDeviceApi.Exists() && DriverData.CrestronSerialDeviceApi.UserAttributes.Exists())
			{
				// Add the static user attributes defined in the driver's json file
				foreach (var attribute in DriverData.CrestronSerialDeviceApi.UserAttributes.Where(attribute => !_userAttributes.ContainsKey(attribute.ParameterId)))
					_userAttributes.Add(attribute.ParameterId, attribute);
			}

			var supportsUnsolicitedFeedback = driverData.CrestronSerialDeviceApi.Api != null && driverData.CrestronSerialDeviceApi.Api.Feedback != null && driverData.CrestronSerialDeviceApi.Api.Feedback.SupportsUnsolicitedFeedback == true;
			if (SupportsFeedback == true && supportsUnsolicitedFeedback == false)
			{
				// Only support fake feedback if the driver supports feedback or if it supports unsolicited feedback.
				// Volume level feedback is faked using a different mechanism and is not affected by this.
				// The controller will still figure out if the specific tpye of feedback is supported before sending any events.
				FakeFeedbackController = new FeedbackController(Logger);
			}
		}

		/// <summary>
		/// Specifies if the driver supports feedback from the device.
		/// <para />If this is false, then the driver will not provide any events for device feedback.
		/// <para />This value is specified by the JSON driver data - the following node must contain CommonFeatureSupport.SupportsFeedback and it must be set to true:
		/// <para />CrestronSerialDeviceApi.DeviceSupport
		/// </summary>
		public bool SupportsFeedback => DriverData.CrestronSerialDeviceApi.DeviceSupport.ContainsKey(CommonFeatureSupport.SupportsFeedback) && DriverData.CrestronSerialDeviceApi.DeviceSupport[CommonFeatureSupport.SupportsFeedback];

		#region Logging

		protected bool InternalEnableStackTrace;

		/// <summary>
		/// Enables logging on the driver.
		/// </summary>
		public bool EnableLogging
		{
			set
			{
				InternalEnableLogging = value;

				if (ConnectionTransport != null)
				{
					ConnectionTransport.EnableLogging = value;
				}
				if (Logger != null)
				{
					Logger.LoggingEnabled = value;
				}
			}
			get { return InternalEnableLogging; }
		}

		/// <summary>
		/// Called by drivers to log a message.
		/// <para />Logged messages will contain the current tick count, driver ID, abstract class type, and the specified message.
		/// </summary>
		/// <param name="message">The message that should be logged</param>
		protected void Log(string message)
		{
			if (!InternalEnableLogging) return;
			message = $"{Environment.TickCount}::{DateTime.Now}::({DriverID}) {AbstractClassType.Name} : {message}";

			if (InternalCustomLogger == null)
				Console.WriteLine(message);
			else
				InternalCustomLogger(message + "\n");
		}

		#endregion

		#region IConnection3 Members

		/// <summary>
		/// Specifies if the driver supportes secure communications with the device.
		/// <para />Refer to IAuthentication2.Required to see if authentication is required to communicate with the device.
		/// <para />This value is specified by the JSON driver data - the following type must not be AuthenticationTypes.NONE
		/// <para />CrestronSerialDeviceApi.Api.Communication.Authentication.Type
		/// </summary>
		public bool IsSecure
		{
			get
			{
				return DriverData.CrestronSerialDeviceApi.Api.Communication.Authentication.Exists()
						&& DriverData.CrestronSerialDeviceApi.Api.Communication.Authentication.Type != AuthenticationTypes.NONE;
			}
		}

		/// <summary>
		/// Specifies the driver is currently connected to the device.
		/// <para /> Ethernet driver will set this to true when the socket state is OK.
		/// <para /> HTTP drivers will set this to true when communication is possible with the device.
		/// <para /> COM and CEC drivers will set this value if they receive any data from the device.
		/// <para /> IR drivers will not set this to true.
		/// </summary>
		public bool Connected
		{
			get => _connected;
			protected set
			{
				if (_connected == value)
					return;

				_connected = value;
				if (EnableLogging)
					Log($"Connected changed - new state: {_connected}");

				if (ConnectedChanged == null)
				{
					if (EnableLogging)
						Log(string.Format("Nothing is subscribed to the ConnectedChanged event"));
					return;
				}

				if (EnableLogging)
					Log("Raising ConnectedChanged event");
				
				ConnectedChanged.Invoke(this, new ValueEventArgs<bool>(_connected));
			}
		}

		/// <summary>
		/// Specifies if the driver supports disconnecting from the device.
		/// <para />This value is specified by the JSON driver data -
		/// the DeviceSupport dictionary must contain CommonFeatureSupport.SupportsDisconnect as a key with a value of true.
		/// <para />CrestronSerialDeviceApi.DeviceSupport
		/// </summary>
		public virtual bool SupportsDisconnect
		{
			get
			{
				return DriverData.CrestronSerialDeviceApi.DeviceSupport.ContainsKey(CommonFeatureSupport.SupportsDisconnect)
						&& DriverData.CrestronSerialDeviceApi.DeviceSupport[CommonFeatureSupport.SupportsDisconnect];
			}
		}

		/// <summary>
		/// This will disconnect the driver from the device, if supported.
		/// <para />Driver developers - this will call ConnectionTransport.Stop()
		/// </summary>
		public virtual void Disconnect()
		{
			if (ConnectionTransport.Exists())
			{
				ConnectionTransport.Stop();
			}
		}

		/// <summary>
		/// Specifies if the driver supports reconnecting to the device.
		/// <para />This value is specified by the JSON driver data -
		/// the DeviceSupport dictionary must contain CommonFeatureSupport.SupportsReconnect as a key with a value of true.
		/// <para />CrestronSerialDeviceApi.DeviceSupport
		/// </summary>
		public virtual bool SupportsReconnect
		{
			get
			{
				return DriverData.CrestronSerialDeviceApi.DeviceSupport.ContainsKey(CommonFeatureSupport.SupportsReconnect)
						&& DriverData.CrestronSerialDeviceApi.DeviceSupport[CommonFeatureSupport.SupportsReconnect];
			}
		}

		/// <summary>
		/// This will disconnect the driver from the device and then connect to it, if supported.
		/// <para />Driver developers - this will call ConnectionTransport.Stop() then ConnectionTransport.Start()
		/// </summary>
		public virtual void Reconnect()
		{
			if (ConnectionTransport.Exists())
			{
				if (ConnectionTransport.IsConnected)
				{
					ConnectionTransport.Stop();
				}
				ConnectionTransport.Start();
			}
		}

		/// <summary>
		/// This will connect the driver to the device, if supported.
		/// <para />Driver developers - this will call ConnectionTransport.Start()
		/// </summary>
		public virtual void Connect()
		{
			if (ConnectionTransport.Exists())
			{
				ConnectionTransport.Start();
			}
		}

		/// <summary>
		/// Event that is raised if the <see cref="Connected"/> property changes.
		/// </summary>
		public event EventHandler<ValueEventArgs<bool>> ConnectedChanged;

		#endregion IConnection3 Members


		private void LoadDriverData() => ConvertJsonFileToDriverData(ReadJsonFile());

		private string ReadJsonFile()
		{
			var dataFile = string.Empty;

			var assembly = AbstractClassType.Assembly;
			var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

			if (!string.IsNullOrEmpty((resourceName)))
			{
				using (var stream = assembly.GetManifestResourceStream(resourceName))
				{
					if (stream != null)
					{
						using (var reader = new StreamReader(stream))
						{
							dataFile = reader.ReadToEnd();
						}
					}
				}
			}
			else
			{
				if (EnableLogging)
					Log("Driver JSON file is missing");
			}
			return dataFile;
		}

		/// <summary>
		/// Maps the device type string in the JSON data to Crestron.RAD.Common.Enums.DeviceTypes
		/// </summary>
		private void ParseDeviceTypeFromJsonFile()
		{
			try
			{
				_deviceType = (Crestron.RAD.Common.Enums.DeviceTypes)Enum.Parse(typeof(Crestron.RAD.Common.Enums.DeviceTypes),
																				DriverData.CrestronSerialDeviceApi.GeneralInformation.DeviceType.Replace(" ", string.Empty),
																				true);
			}
			catch (Exception e)
			{
				if (EnableLogging)
				{
					Logger.Error($"ParseDeviceTypeFromJsonFile - Unable to parse device type: {e.Message}");
				}
			}
		}

		#region Console Commands

		private void DriverConsoleCommandCallback(ABasicDriver.ConsoleCommandType command, string args)
		{
			switch (command)
			{
				case ConsoleCommandType.General:
					LoggingToggle(args);
					break;
				case ConsoleCommandType.LoggingLevel:
					SetLoggingLevel(args);
					break;
			}
		}

		private void SetLoggingLevel(string args)
		{
			if (Logger != null &&
				IsValidDriverID(args))
			{
				var argsSplit = args.Split(' ');

				if (argsSplit.Length >= 2)
				{
					if (argsSplit[1].Equals(_debugLoggingLevelParamater, StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.CurrentLevel = LoggingLevel.Debug;
						Console.WriteLine("Logging level set to Debug on driver ID {0}", DriverID);
					}
					else if (argsSplit[1].Equals(_warningLoggingLevelParamater, StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.CurrentLevel = LoggingLevel.Warning;
						Console.WriteLine("Logging level set to Warning on driver ID {0}", DriverID);
					}
					else if (argsSplit[1].Equals(_errorLoggingLevelParamater, StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.CurrentLevel = LoggingLevel.Error;
						Console.WriteLine("Logging level set to Error on driver ID {0}", DriverID);
					}
				}
			}
		}

		private void LoggingToggle(string args)
		{
			if (IsValidCommandParameter(args) && IsValidDriverID(args))
			{
				EnableLogging = GetStateFromCommandParameter(args);
			}
		}

		private bool IsValidDriverID(string args)
		{
			var idValue = args.Split(' ')[0];

			try
			{
				var id = int.Parse(idValue);

				if (id == DriverID || id == 0)
				{
					return true;
				}
			}
			catch
			{ }

			return false;
		}

		private bool GetStateFromCommandParameter(string args)
		{
			var state = args.Split(' ')[1];

			if (state.Equals("on", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return false;
		}

		private bool IsValidCommandParameter(string args)
		{
			if (args.Contains(' '))
			{
				var arguments = args.Split(' ');

				if (arguments[1].Equals("on", StringComparison.OrdinalIgnoreCase) ||
					arguments[1].Equals("off", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		#endregion Console Commands
	}
}
