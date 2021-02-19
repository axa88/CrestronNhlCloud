﻿using System;
using System.Collections.Generic;

using Crestron.RAD.Common;
using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.CloudReporting;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.ExtensionMethods;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using NhlApiShared;
using NhlApiShared.Common;

using CrestronNhlCloud.Transport;


using static NhlApiShared.Common.UiObjects;


namespace CrestronNhlCloud
{
	public class NhlExtension : AExtensionDevice, ICloudConnected, IApplication, IPlatform, ISettings
	{
		public HttpTransport HttpTransport;
		private Logic _logic;

		public NhlExtension()
		{
			PrintLine("RAD EX: cstr start");
			try
			{

			}
			catch (Exception e)
			{
				CatchPrint(e);
				throw new Exception("Bad Constructor");
			}

			PrintLine("RAD EX: cstr end");
		}

		#region Implementation of ICloudConnected
		public void Initialize()
		{
			PrintLine("RAD EXT: ini start");

			try
			{
				HttpTransport = new HttpTransport();
				_logic = new Logic(this);
				_logic.TeamGoalScored += () => TeamGoalScored?.Invoke();
				_logic.Initialize();

			}
			catch (Exception e) { CatchPrint(e); }

			PrintLine("RAD EX: ini end");
		}
		#endregion

		#region Implementation of IApplication
		[ProgrammableEvent ("^PreGameStarted")]
		public event Action PreGameStarted;

		[ProgrammableEvent ("^GameStarted")]
		public event Action GameStarted;

		[ProgrammableEvent ("^PuckDropped")]
		public event Action PuckDropped;

		[ProgrammableEvent ("^OverTimeStarted")]
		public event Action OverTimeStarted;

		[ProgrammableEvent ("^GameEnded")]
		public event Action GameEnded;

		[ProgrammableEvent ("^TeamGoalScored")]
		public event Action TeamGoalScored;

		[ProgrammableEvent ("^OpponentGoalScored")]
		public event Action OpponentGoalScored;
		#endregion

		#region Implementation of IPlatform
		public void UpdateUi() => Commit();
		public void Connect(bool state) => Connected = state;
		public void PrintLine(string message) => Log(message);
		public PropertyValue<T> CreateProperty<T>(string key, DevicePropertyType type, IEnumerable<IPropertyAvailableValue> availableValues = null)
			=> CreateProperty<T>(availableValues == null ? new PropertyDefinition(key, key, type) : new PropertyDefinition(key, key, type, availableValues));
		#endregion

		#region Implementation of ISettings
		public bool GetBoolSetting(string key) => Convert.ToBoolean(GetSetting(key));
		public void SetBoolSetting(string key, bool value) => SaveSetting(key, value);
		public ushort GetNumberSetting(string key) => Convert.ToUInt16(GetSetting(key));
		public void SetNumberSetting(string key, ushort value) => SaveSetting(key, value);
		public string GetStringSetting(string key) => GetSetting(key).ToString();
		public void SetStringSetting(string key, string value) => SaveSetting(key, value);
		#endregion

		#region Overrides of AExtensionDevice
		protected override IOperationResult DoCommand(string command, string[] parameters)
		{
			PrintLine(nameof(DoCommand));
			PrintLine($"command: {command}");
			foreach (var parameter in parameters)
				PrintLine($"parameters: {parameter}");

			switch (command)
			{
				case TileActionRoom:
					try { PrintLine($"{nameof(TileActionRoom)}"); }
					catch (Exception e) { CatchPrint(e); }
					break;

				default:
					PrintLine("NHL: Unhandled command: " + command);
					break;
			}

			return new OperationResult(OperationResultCode.Success);
		}

		protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
		{
			PrintLine(nameof(SetDriverPropertyValue));
			PrintLine($"propertyKey: {propertyKey}{Environment.NewLine}value: {value}");

			try
			{
				switch (propertyKey)
				{
					case SelectorButtonValueTeam:
						// if team changes
						var old = ((PropertyValue<ushort>)_logic.Properties[SelectorButtonValueTeam]).Value;

						// set button value to selected value
						_logic.ChangeProperty(propertyKey, value);

						// store current team
						SetNumberSetting(nameof(DefaultTeamKey), ((PropertyValue<ushort>)_logic.Properties[SelectorButtonValueTeam]).Value);

						// if team changed update
						if (Convert.ToUInt16(value) != old)
						{
							PrintLine("team changed");
							_logic.PollTimerCallback(this);
						}

						break;
				}

				return new OperationResult(OperationResultCode.Success);
			}
			catch (Exception exception)
			{
				CatchPrint(exception);
				return new OperationResult(OperationResultCode.Error);
			}
		}

		protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
		{
			PrintLine($"{nameof(SetDriverPropertyValue)} obj overload. id: {objectId}, prop key: {propertyKey} val: {value}");
			return new OperationResult(OperationResultCode.Success);
		}
		#endregion

		public void CatchPrint(Exception exception)
		{
			PrintLine(exception.Message);
			PrintLine(exception.StackTrace);
		}

		#region IDisposable
		public override void Dispose()
		{
			PrintLine("Ext Dispose");

			HttpTransport?.Dispose();
			_logic?.Dispose();
			base.Dispose();
		}
		#endregion
	}
}
