using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Logging;

using Newtonsoft.Json;


namespace TestConsole.Portability
{
	public abstract class AExtensionDevice : IDisposable
	{
		#region Fields
		private const string ClassNameForLogging = nameof(AExtensionDevice);
		private const string UiDefinitionsFolderName = "uidefinitions";
		private const string TranslationsFolderName = "translations";
		private const string XmlFileExtension = ".xml";
		private const string JsonFileExtension = ".json";
		private const string EnglishUsCulture = "en-US";
		public const string ProgrammingFolderName = "programming";
		public const string ProgrammingFileName = "programming.json";

		private string _uiDefinitionCache;
		private DateTime _uiDefinitionLastModifiedTime;

		private ReadOnlyCollection<string> _supportedCulturesCache;
		private DateTime _supportedCulturesLastModifiedTime;

		private readonly Dictionary<string, ReadOnlyDictionary<string, string>> _translationsCache = new Dictionary<string, ReadOnlyDictionary<string, string>>();
		private readonly Dictionary<string, DateTime> _translationsLastModifiedTime = new Dictionary<string, DateTime>();

		/// <summary>
		/// Contains the definitions for all object classes
		/// </summary>
		private readonly List<IClassDefinition> _classDefinitions;
		//private readonly CCriticalSection _classDefinitionsLock = new CCriticalSection();
		private readonly object _classDefinitionsLock = new object();

		/// <summary>
		/// Contains the definitions for all root properties by key
		/// </summary>
		private readonly Dictionary<string, IPropertyDefinition> _rootPropertyDefinitions;
		private readonly object _rootPropertyDefinitionsLock = new object();

		/// <summary>
		/// Contains all of the property values by id
		/// </summary>
		private readonly Dictionary<string, IPropertyValue> _propertyValues;
		private readonly object _propertyValuesLock = new object();

		/// <summary>
		/// Contains all of the property values that have changed since the last time <see cref="IExtensionDevice.PropertyValuesChanged"/> was raised.
		/// </summary>
		private readonly Dictionary<string, IPropertyValue> _pendingPropertyValueChanges;
		private readonly object _pendingPropertyValueChangesLock = new object();

		private EventHandler _propertyDefinitionChangedEventHandler;
		private EventHandler<PropertyValuesEventArgs> _propertyValuesChangedEventHandler;
		private EventHandler _uiDefinitionChangedEventHandler;
		private EventHandler _supportedCulturesChangedEventHandler;
		private EventHandler _languageTranslationsChangedEventHandler;
		#endregion

		private bool _connected;

		protected AExtensionDevice()
		{
			if (EnableLogging)
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Debug, ClassNameForLogging, "Constructor", "Method called.");

			_classDefinitions = new List<IClassDefinition>();
			_rootPropertyDefinitions = new Dictionary<string, IPropertyDefinition>();
			_propertyValues = new Dictionary<string, IPropertyValue>();
			_pendingPropertyValueChanges = new Dictionary<string, IPropertyValue>();
		}

		/// <summary>
		/// Create a property that is a value type such as int, string, bool, etc.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the property. This must match the <see cref="DevicePropertyType"/> of the provided <see cref="PropertyDefinition"/>.
		/// </typeparam>
		/// <param name="propertyDefinition">The definition of the property.</param>
		/// <returns>A new <see cref="PropertyValue"/></returns>
		protected PropertyValue<T> CreateProperty<T>(PropertyDefinition propertyDefinition)
		{
			const string methodNameForLogging = "CreateProperty";

			if (propertyDefinition == null)
			{
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Error, ClassNameForLogging, methodNameForLogging, "Property definition can't be null.");
				return null;
			}

			// Verify the DevicePropertyType passed in matches the provided type T
			if (!ExtensionDevicePropertyHelper.VerifyValueType<T>(propertyDefinition.Type))
			{
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Error, ClassNameForLogging, methodNameForLogging,
					"The provided value type T does not match the provided DevicePropertyType in the property definition.");
				return null;
			}

			try
			{
				//_rootPropertyDefinitionsLock.Enter();
				lock (_rootPropertyDefinitionsLock)
				{
					// If a property with the same key already exists log error and return
					if (_rootPropertyDefinitions.ContainsKey(propertyDefinition.Key))
					{
						ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Error, ClassNameForLogging, methodNameForLogging, $"A property with the provided key already exists. Key = '{propertyDefinition.Key}'");
						return null;
					}
				}
			}
			finally
			{
				//_rootPropertyDefinitionsLock.Leave();
			}

			if (EnableLogging)
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Debug, ClassNameForLogging, methodNameForLogging, $"Creating property. Key = '{propertyDefinition.Key}', Type = '{propertyDefinition.Type}'");

			// Create the device property value
			var propertyValue = new PropertyValue<T>(propertyDefinition.Key, propertyDefinition.Type, Log);

			// Subscribe to events on the property value
			propertyValue.PropertyChanged -= PropertyValue_PropertyChanged;
			propertyValue.PropertyChanged += PropertyValue_PropertyChanged;

			// Subscribe to definition changed event
			propertyDefinition.DefinitionChanged -= PropertyDefinition_DefinitionChanged;
			propertyDefinition.DefinitionChanged += PropertyDefinition_DefinitionChanged;

			AddRootPropertyToCollection(propertyDefinition);
			AddPropertyValueToCollection(propertyValue);

			return propertyValue;
		}

		#region IExtensionDevice Members Implemented By Driver Developer

		/// <summary>
		/// Sends a command to the device.
		/// </summary>
		/// <param name="command">The command to send.</param>
		/// <param name="parameters">The commands parameters.</param>
		protected abstract IOperationResult DoCommand(string command, string[] parameters);

		/// <summary>
		/// Set the value of a property.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="propertyKey">The property key.</param>
		/// <param name="value">The value to set.</param>
		protected abstract IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value);

		/// <summary>
		/// Set the value of a property on an object.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="objectId">The id of the object.</param>
		/// <param name="propertyKey">The property key.</param>
		/// <param name="value">The value to set.</param>
		protected abstract IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value);

		#endregion

		#region Device Properties

		private PropertyValue TryGetPropertyValueById(string propertyId)
		{
			try
			{
				//_propertyValuesLock.Enter();
				lock (_propertyValuesLock)
					return _propertyValues.TryGetValue(propertyId, out var property) ? property as PropertyValue : null;
			}
			finally
			{
				//_propertyValuesLock.Leave();
			}
		}

		private Dictionary<string, IPropertyDefinition> GetAllPropertyDefinitions()
		{
			Dictionary<string, IPropertyDefinition> returnDic;

			try
			{
				//_rootPropertyDefinitionsLock.Enter();
				lock (_rootPropertyDefinitionsLock)
				{
					// Get all root properties
					returnDic = _rootPropertyDefinitions.ToDictionary(property => property.Key, property => property.Value);
				}
			}
			finally
			{
				//_rootPropertyDefinitionsLock.Leave();
			}

			try
			{
				//_classDefinitionsLock.Enter();
				lock (_classDefinitionsLock)
				{
					// Get all property definitions for each class
					foreach (var property in _classDefinitions.Select(classDef => classDef.Properties).SelectMany(classProperties => classProperties))
					{
						// If there is a duplicate key found log an error and do not add it to the return value
						if (returnDic.ContainsKey(property.Key))
						{
							ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Error, $"Duplicate key found in property definitions, all property definition keys must be unique across all objects. Key = '{property.Key}'");
							continue;
						}

						returnDic.Add(property.Key, property.Value);
					}
				}
			}
			finally
			{
				//_classDefinitionsLock.Leave();
			}

			return returnDic;
		}

		private void AddPropertyValueToCollection(IPropertyValue propertyValue)
		{
			try
			{
				//_propertyValuesLock.Enter();
				lock (_propertyValuesLock)
					_propertyValues.Add(propertyValue.Id, propertyValue);
			}
			finally
			{
				//_propertyValuesLock.Leave();
			}

			QueuePropertyValueChangedEvent(propertyValue);
		}

		private void RemovePropertyValueFromCollection(string objectId)
		{
			try
			{
				//_propertyValuesLock.Enter();
				lock (_propertyValuesLock)
					_propertyValues.Remove(objectId);
			}
			finally
			{
				//_propertyValuesLock.Leave();
			}
		}

		private void AddRootPropertyToCollection(IPropertyDefinition propertyDefinition)
		{
			try
			{
				//_rootPropertyDefinitionsLock.Enter();
				lock (_rootPropertyDefinitionsLock)
					_rootPropertyDefinitions.Add(propertyDefinition.Key, propertyDefinition);
			}
			finally
			{
				//_rootPropertyDefinitionsLock.Leave();
			}

			RaisePropertyDefinitionChangedEvent();
		}

		private void AddClassToCollection(IClassDefinition classDefinition)
		{
			try
			{
				//_classDefinitionsLock.Enter();
				lock (_classDefinitionsLock)
				{
					_classDefinitions.Add(classDefinition);
					RaisePropertyDefinitionChangedEvent();
				}
			}
			finally
			{
				//_classDefinitionsLock.Leave();
			}

			RaisePropertyDefinitionChangedEvent();
		}

		#endregion

		#region Events
		/// <summary>
		/// Sends out an event with all of the modified property values. This allows property values to be updated in batches.
		/// </summary>
		protected void Commit()
		{
			if (EnableLogging)
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Debug, ClassNameForLogging, "Commit", "Method called");

			RaisePropertyValuesChangedEvent();
		}

		/// <summary>
		/// Queue a property value changed event whenever a property is modified, added, or removed.
		/// If the property is added or removed also queue the property that it was added or removed from.
		/// </summary>
		/// <param name="value"></param>
		private void QueuePropertyValueChangedEvent(IPropertyValue value)
		{
			try
			{
				//_pendingPropertyValueChangesLock.Enter();
				lock (_pendingPropertyValueChangesLock)
					_pendingPropertyValueChanges[value.Id] = value;
			}
			finally
			{
				//_pendingPropertyValueChangesLock.Leave();
			}
		}

		/// <summary>
		/// This method should only ever be called from <see cref="Commit"/>.
		/// All property value changes should be queued using <see cref="QueuePropertyValueChangedEvent"/>.
		/// </summary>
		private void RaisePropertyValuesChangedEvent()
		{
			var handler = _propertyValuesChangedEventHandler;

			if (handler == null)
			{
				if (EnableLogging)
					ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Warning, "PropertyValuesChanged event is null");
				return;
			}

			IEnumerable<IPropertyValue> pendingPropertyValues;

			try
			{
				//_pendingPropertyValueChangesLock.Enter();
				lock (_pendingPropertyValueChangesLock)
				{
					if (_pendingPropertyValueChanges.Count == 0)
						return;

					pendingPropertyValues = _pendingPropertyValueChanges.Values.ToList();

					_pendingPropertyValueChanges.Clear();
				}
			}
			finally
			{
				//_pendingPropertyValueChangesLock.Leave();
			}

			if (EnableLogging)
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Debug, $"Raising property values changed event for {pendingPropertyValues.Count()} pending change(s)");

			handler.Invoke(this, new PropertyValuesEventArgs(pendingPropertyValues));
		}

		private void RaisePropertyDefinitionChangedEvent()
		{
			_propertyDefinitionChangedEventHandler?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region Event Handlers

		private void PropertyValue_PropertyChanged(object sender, EventArgs args)
		{
			if (!(sender is IPropertyValue propertyValue))
				return;

			QueuePropertyValueChangedEvent(propertyValue);
		}

		private void PropertyDefinition_DefinitionChanged(object sender, EventArgs eventArgs)
		{
			if (!(sender is IPropertyDefinition definition))
				return;

			if (EnableLogging)
				ExtensionDeviceLogHelper.LogMessage(Log, LogLevel.Debug, $"Property definition changed. Key = '{definition.Key}'");

			RaisePropertyDefinitionChangedEvent();
		}

		#endregion

		#region ABaseDriver

		private Dictionary<string, object> _driverSettings = new Dictionary<string, object>();
		private string _driverSettingsString;

		private EventHandler<DriverSettingsEventArgs> _driverSettingsChangedEventHandler;

		private JsonSerializerSettings _driverDataSerializer = new JsonSerializerSettings()
																{
																	PreserveReferencesHandling = PreserveReferencesHandling.Objects,
																	TypeNameHandling = TypeNameHandling.All
																};

		/// <summary>
		/// Save a setting for this device by key which will be persisted through reboots and power loss.
		/// <para>
		/// This can be useful for devices that don't remember their own settings or can be used to return a device
		/// to its previous state after a power failure/reboot/etc.
		/// </para>
		/// <para>
		/// To retreive the value of the setting use <see cref="GetSetting"/>.
		/// </para>
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		protected void SaveSetting(string key, object value)
		{
			// Save the setting and overwrite any existing setting for the provided key
			_driverSettings[key] = value;

			var strDriverSettingsData = JsonConvert.SerializeObject(_driverSettings, Formatting.None, _driverDataSerializer);
			_driverSettingsString = strDriverSettingsData;

			RaiseDriverSettingsChangedEvent(strDriverSettingsData);
		}

		/// <summary>
		/// Erase the value of a saved setting from memory.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		protected void DeleteSetting(string key)
		{
			if (!_driverSettings.ContainsKey(key))
				return;

			_driverSettings.Remove(key);
			_driverSettingsString = JsonConvert.SerializeObject(_driverSettings, Formatting.None, _driverDataSerializer);
			RaiseDriverSettingsChangedEvent(_driverSettingsString);
		}

		/// <summary>
		/// Get the value of a setting that has been saved.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <returns>The value of the setting. Null if the key is not found.</returns>
		protected object GetSetting(string key)
		{
			object settingValue;
			return _driverSettings.TryGetValue(key, out settingValue) ? settingValue : null;
		}

		private void RaiseDriverSettingsChangedEvent(string driverSettings)
		{
			if (_driverSettingsChangedEventHandler == null)
				return;

			_driverSettingsChangedEventHandler.Invoke(this, new DriverSettingsEventArgs(driverSettings));
		}

		#region IConnection3 Members
		protected static void Log(string message) => Console.WriteLine($"{DateTime.Now.Ticks}::{DateTime.Now}::{message}");

		/// <summary>
		/// Specifies the driver is currently connected to the device.
		/// <para /> Ethernet driver will set this to true when the socket state is OK.
		/// <para /> HTTP drivers will set this to true when communication is possible with the device.
		/// <para /> COM and CEC drivers will set this value if they receive any data from the device.
		/// <para /> IR drivers will not set this to true.
		/// </summary>
		protected bool Connected
		{
			get => _connected;
			set
			{
				if (_connected != value)
				{
					_connected = value;

					if (EnableLogging)
						Log($"Connected changed - new state: {_connected}");

					if (ConnectedChanged != null)
					{
						if (EnableLogging)
							Log($"Raising {nameof(ConnectedChanged)} event");

						ConnectedChanged.Invoke(this, new ValueEventArgs<bool>(_connected));
					}
					else
					{
						if (EnableLogging)
							Log($"Nothing is subscribed to the {nameof(ConnectedChanged)} event");
					}
				}
			}
		}

		/// <summary>
		/// Event that is raised if the <see cref="Connected"/> property changes.
		/// </summary>
		public event EventHandler<ValueEventArgs<bool>> ConnectedChanged;

		#endregion IConnection3 Members
		#endregion

		#region IBasicLogger

		/// <summary>
		/// Property to enable Logging statements.
		/// </summary>
		public bool EnableLogging { get; set; }

		#endregion

		#region IDisposable
		public virtual void Dispose()
		{

		}
		#endregion
	}
}
