using System;
using System.Collections.Generic;

using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using NhlApiShared;
using NhlApiShared.Common;

using TestConsole.Portability;


namespace TestConsole
{
	public class NhlExtension :  AExtensionDevice, ICloudConnected, IApplication, IPlatform, ISettings
	{
		private Logic _logic;

		public NhlExtension()
		{
			PrintLine("RAD EX: cstr start");
			try
			{

			}
			catch (Exception e)
			{
				PrintLine(e.Message);
				throw new Exception(e.StackTrace);
			}

			PrintLine("RAD EX: cstr end");
		}

		#region This Only // replaced with a UI
		internal event Action<string> StatusChanged;

		#endregion

		#region IDisposable
		public override void Dispose()
		{
			PrintLine("Ext Dispose");

			_logic?.Dispose();
			base.Dispose();
		}
		#endregion

		#region Implementation of ICloudConnected
		public void Initialize()
		{
			PrintLine("RAD EXT: ini");

			try
			{
				_logic = new Logic(this);
				_logic.Initialize();
			}
			catch (Exception e) { _logic.CatchPrint(e); }

			PrintLine("RAD EX: ini end");
		}
		#endregion

		#region Implementation of IApplication
		[ProgrammableEvent ("^PreGameStarted")]
		public event Action PreGameStarted;

		[ProgrammableEvent ("^PuckDropped")]
		public event Action PuckDropped;

		[ProgrammableEvent ("^CriticalGamePlayStarted")]
		public event Action CriticalGamePlayStarted;

		[ProgrammableEvent ("^GameEnded")]
		public event Action GameEnded;

		[ProgrammableEvent ("^TeamGoalScored")]
		public event Action TeamGoalScored;

		[ProgrammableEvent ("^OpponentGoalScored")]
		public event Action OpponentGoalScored;

		public void OnPreGameStarted() => PreGameStarted?.Invoke();
		public void OnOnPuckDropped() => PuckDropped?.Invoke();
		public void OnCriticalGamePlayStarted() => CriticalGamePlayStarted?.Invoke();
		public void OnGameEnded() { GameEnded?.Invoke(); }
		public void OnTeamGoalScored() => TeamGoalScored?.Invoke();
		public void OnOpponentGoalScored() => OpponentGoalScored?.Invoke();
		#endregion

		#region Implementation of IPlatform
		public void UpdateUi() => Commit();
		public void Connect(bool state) => Connected = state;
		public void PrintLine(string message) => Log(message);
		public PropertyValue<T> CreateProperty<T>(string key, DevicePropertyType type, IEnumerable<IPropertyAvailableValue> availableValues = null)
			=> CreateProperty<T>(availableValues == null ? new PropertyDefinition(key, key, type) : new PropertyDefinition(key, key, type, availableValues));
		public PropertyValue<T> CreateProperty<T>(string key, DevicePropertyType type, ushort min, ushort max, ushort step)
			=>  CreateProperty<T>(new PropertyDefinition(key, key, type, min, max, step));
		public void ConsoleReader(string args) => _logic.ConsoleReader(args);
		#endregion

		#region Implementation of ISettings
		public bool GetBoolSetting(string key) => Convert.ToBoolean(GetSetting(key));
		public void SetBoolSetting(string key, bool value) => SaveSetting(key, value);
		//public ushort GetNumberSetting(string key) => Convert.ToUInt16(GetSetting(key));
		public ushort GetNumberSetting(string key) => 1;
		public void SetNumberSetting(string key, ushort value) => SaveSetting(key, value);
		public string GetStringSetting(string key) => GetSetting(key).ToString();
		public void SetStringSetting(string key, string value) => SaveSetting(key, value);
		#endregion

		#region Overrides of AExtensionDevice
		protected override IOperationResult DoCommand(string command, string[] parameters) => _logic.DoCommand(command, parameters);
		protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value) => _logic.SetDriverPropertyValue(propertyKey, value);
		protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value) => _logic.SetDriverPropertyValue(objectId, propertyKey, value);
		#endregion
	}
}
