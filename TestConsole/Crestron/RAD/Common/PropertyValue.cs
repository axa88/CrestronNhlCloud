using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
//using Crestron.SimplSharp;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.DeviceTypes.ExtensionDevice
{
	/// <summary>
	/// Represents the intance of a <see cref="PropertyValue"/>.
	/// <para>For value types this is a <see cref="PropertyValue{T}"/></para>
	/// <para>For objects this is a <see cref="ObjectValue"/></para>
	/// <para>For object lists this is a <see cref="ObjectList"/></para>
	/// </summary>
	public class PropertyValue : IPropertyValue
	{
		#region Fields

		private bool _enabled;
		private bool _isValueAvailable;

		protected readonly Action<string> Logger;
		protected string ClassNameForLogging = "PropertyValue";

		#endregion

		#region Constructor

		/// <summary>
		/// Create a new property value.
		/// </summary>
		/// <param name="definitionKey">The property definition key.</param>
		/// <param name="type">The type of the properties value.</param>
		/// <param name="logger">Method for logging.</param>
		protected PropertyValue(string definitionKey, DevicePropertyType type, Action<string> logger)
		{
			Id = Guid.NewGuid().ToString();
			DefinitionKey = definitionKey;
			Type = type;
			Logger = logger;
			ParentIds = new List<string>();

			_enabled = true;
			_isValueAvailable = true;
		}

		#endregion

		#region Events

		internal event EventHandler PropertyChanged;

		#endregion

		#region IPropertyValue Members

		/// <summary>
		/// A Guid used to uniquely identify this property value.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Indicates the definition of this property value. <see cref="IPropertyDefinition.Key"/>.
		/// </summary>
		public string DefinitionKey { get; private set; }

		/// <summary>
		/// Represents the type of this property value. The type is based on the <see cref="PropertyDefinition.Type"/>.
		/// </summary>
		public DevicePropertyType Type { get; private set; }

		/// <summary>
		/// Indicates if the property value is currently enabled.
		/// If false, the value will not be selectable or editable by the user.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				RaisePropertyChangedEvent();
			}
		}

		/// <summary>
		/// Indicates if the property Value is currently available.
		/// If false, the current value of <see cref="IPropertyValue{T}.Value"/> will be ignored.
		/// </summary>
		public bool IsValueAvailable
		{
			get { return _isValueAvailable; }
			set
			{
				if (_isValueAvailable == value)
					return;

				_isValueAvailable = value;
				RaisePropertyChangedEvent();
			}
		}

		bool IPropertyValue.IsRootProperty
		{
			get { return ParentIds.Count == 0; }
		}

		ReadOnlyCollection<string> IPropertyValue.MemberIds
		{
			get
			{
				var members = GetMembers();
				return members == null ? null : new ReadOnlyCollection<string>(members);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The <see cref="PropertyValue.Id"/> of the parent or parents if applicable. Empty, if this property does not have a parent.
		/// </summary>
		internal List<string> ParentIds { get; set; }

		#endregion

		#region Non-Public Methods

		/// <summary>
		/// Gets the children of the property. For Objects these are the properties that make up the object
		/// and for Lists these are the objects that are currently in the list.
		/// </summary>
		/// <returns></returns>
		protected virtual List<string> GetMembers()
		{
			return null;
		}

		/// <summary>
		/// Event to be raised whenever a public property on a <see cref="PropertyValue"/> changes.
		/// </summary>
		protected void RaisePropertyChangedEvent()
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}

	/// <summary>
	/// Represents the intance of a <see cref="PropertyValue"/> that is of a value type such as int, string, or bool.
	/// </summary>
	public class PropertyValue<T> : PropertyValue, IPropertyValue<T>
	{
		private T _value;

		/// <summary>
		/// Create a property value for a simple type (string, int, bool, etc.).
		/// </summary>
		/// <param name="definitionKey">The property definition key.</param>
		/// <param name="type">The type of T</param>
		/// <param name="logger">Method for logging.</param>
		internal PropertyValue(string definitionKey, DevicePropertyType type, Action<string> logger)
			: base(definitionKey, type, logger)
		{
			// Verify the DevicePropertyType passed in matches the provided type T
			if (!ExtensionDevicePropertyHelper.VerifyValueType<T>(type))
				throw new ArgumentException("The provided value type T does not match the provided DevicePropertyType");

			ClassNameForLogging = "Property";
		}

		/// <summary>
		/// The value of the property.
		/// </summary>
		public T Value
		{
			get { return _value; }
			set
			{
				if (EqualityComparer<T>.Default.Equals(_value, value))
					return;

				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Debug, ClassNameForLogging, "Value",
					string.Format("'{0}' value changed from '{1}' to '{2}'. Id = '{3}'", DefinitionKey, _value, value, Id));

				_value = value;
				RaisePropertyChangedEvent();
			}
		}
	}

	/// <summary>
	/// Represents the intance of a <see cref="PropertyValue"/> that is of type <see cref="DevicePropertyType.Object"/>.
	/// </summary>
	public class ObjectValue : PropertyValue, IPropertyObjectValue
	{
		/// <summary>
		/// Create a property value for an object.
		/// </summary>
		/// <param name="className">The name of the class that this value belongs too.</param>
		/// <param name="logger">Method for logging.</param>
		internal ObjectValue(string className, Action<string> logger) : this(string.Empty, className, logger)
		{
		}

		/// <summary>
		/// Create a property value for an object.
		/// </summary>
		/// <param name="definitionKey">
		/// The key of this property's property definition if it has one. Root level objects don't
		/// have a property definition, only a class definition.
		/// </param>
		/// <param name="className">The name of the class that this value belongs too.</param>
		/// <param name="logger">Method for logging.</param>
		internal ObjectValue(string definitionKey, string className, Action<string> logger)
			: base(definitionKey, DevicePropertyType.Object, logger)
		{
			SubType = className;
			ObjectProperties = new Dictionary<string, PropertyValue>();
			ClassNameForLogging = "ObjectValue";
		}

		/// <summary>
		/// The class type of the object. See <see cref="ClassDefinition.ClassName"/>.
		/// </summary>
		public string SubType { get; private set; }

		/// <summary>
		/// Dictionary containing the ids of the properties of this object by property key.
		/// Key = <see cref="IPropertyValue.DefinitionKey"/>
		/// Value = <see cref="IPropertyValue.Id"/>
		/// </summary>
		internal Dictionary<string, PropertyValue> ObjectProperties { get; set; }

		/// <summary>
		/// Get the value of a property.
		/// <para>
		/// Applies to properties that are value types such as int, string, bool, etc.
		/// </para>
		/// <para>
		/// See <see cref="PropertyValue{T}.Value"/> to get an set the actual value of this property.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type of the properties value.</typeparam>
		/// <param name="propertyKey">The key of the property.</param>
		/// <returns>The <see cref="PropertyValue{T}"/> containing the value.</returns>
		public PropertyValue<T> GetValue<T>(string propertyKey)
		{
			const string methodNameForLogging = "GetValue";

			// Get the property value with the provided key
			PropertyValue propertyToGet;
			if (!ObjectProperties.TryGetValue(propertyKey, out propertyToGet))
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key was not found on this object. ObjectId = '{0}', Key = '{1}'", Id, propertyKey));
				return null;
			}

			if (propertyToGet == null)
				return null;

			// Verify that the value they are trying to get matches the type of the value on this property
			if (!ExtensionDevicePropertyHelper.VerifyValueType<T>(propertyToGet.Type))
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					"The type of T does not match the type of the property you are trying to get.");
				return null;
			}

			// Verify that the value they are getting is actually of a type that contains a value
			var internalPropertyToGet = propertyToGet as PropertyValue<T>;
			if (internalPropertyToGet == null)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key does not have a value. Id = '{0}', Key = '{1}'", Id, propertyKey));
				return null;
			}

			return internalPropertyToGet;
		}

		/// <summary>
		/// Get the value of a property where the property is an object.
		/// <para>
		/// Applies to properties that are of type <see cref="DevicePropertyType.Object"/>.
		/// </para>
		/// </summary>
		/// <param name="propertyKey">The key of the property.</param>
		/// <returns>The object value of the property.</returns>
		public ObjectValue GetObjectValue(string propertyKey)
		{
			const string methodNameForLogging = "GetObjectValue";

			// Get the property value with the provided key
			PropertyValue propertyToGet;
			if (!ObjectProperties.TryGetValue(propertyKey, out propertyToGet))
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key was not found on this object. ObjectId = '{0}', Key = '{1}'", Id, propertyKey));
				return null;
			}

			if (propertyToGet == null)
				return null;

			// Verify the property value they are trying to get is an object
			if (propertyToGet.Type != DevicePropertyType.Object)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key is not of type Object. Id = '{0}'", Id));
				return null;
			}

			return propertyToGet as ObjectValue;
		}

		/// <summary>
		/// Get the value of a property where the property is an object list.
		/// <para>
		/// Applies to properties that are of type <see cref="DevicePropertyType.ObjectList"/>.
		/// </para>
		/// </summary>
		/// <param name="propertyKey">The key of the property.</param>
		/// <returns>The list value of the property.</returns>
		public ObjectList GetListValue(string propertyKey)
		{
			const string methodNameForLogging = "GetListValue";

			// Get the property value with the provided key
			PropertyValue propertyToGet;
			if (!ObjectProperties.TryGetValue(propertyKey, out propertyToGet))
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key was not found on this object. ObjectId = '{0}', Key = '{1}'", Id, propertyKey));
				return null;
			}

			if (propertyToGet == null)
				return null;

			// Verify the property value they are trying to get is an object list
			if (propertyToGet.Type != DevicePropertyType.ObjectList)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key is not of type ObjectList. Id = '{0}'", Id));
				return null;
			}

			return propertyToGet as ObjectList;
		}

		/// <summary>
		/// Sets the value of a property where the property is an object.
		/// <para>
		/// Applies to properties that are of type <see cref="DevicePropertyType.Object"/>.
		/// </para>
		/// <para>
		/// The object provided must be the same class as the property being set. See <see cref="ClassDefinition.ClassName"/>.
		/// </para>
		/// </summary>
		/// <param name="propertyKey">The key of the property.</param>
		/// <param name="value">The object value.</param>
		public void SetObjectValue(string propertyKey, ObjectValue value)
		{
			const string methodNameForLogging = "SetObjectValue";

			// Get the property value that is being modified
			PropertyValue propertyToSet;
			if (!ObjectProperties.TryGetValue(propertyKey, out propertyToSet))
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key was not found on this object. ObjectId = '{0}', Key = '{1}'", Id, propertyKey));
				return;
			}

			if (propertyToSet == null)
				return;

			// Verify the property value being set is an object
			if (propertyToSet.Type != DevicePropertyType.Object)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key is not of type Object. Id = '{0}'", Id));
				return;
			}

			var internalPropertyToSet = (ObjectValue)propertyToSet;

			// Verify that the value provided and the value being set are of the same type
			if (internalPropertyToSet.SubType != value.SubType)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format(
						"The value provided and the property value being set are not of the same class type. ValueProvidedType = '{0}', ValueBeingSetType = '{1}'",
						value.SubType, internalPropertyToSet.SubType));
				return;
			}

			// Take the member ids of the value passed in and assign them to the property on this object
			internalPropertyToSet.ObjectProperties = value.ObjectProperties;

			RaisePropertyChangedEvent();
		}

		/// <summary>
		/// Sets the value of a property where the property is an object list.
		/// <para>
		/// Applies to properties that are of type <see cref="DevicePropertyType.ObjectList"/>.
		/// </para>
		/// </summary>
		/// <param name="propertyKey">The key of the property.</param>
		/// <param name="value">The object list value.</param>
		public void SetListValue(string propertyKey, ObjectList value)
		{
			const string methodNameForLogging = "SetListValue";

			// Get the property value that is being modified
			PropertyValue propertyToSet;
			if (!ObjectProperties.TryGetValue(propertyKey, out propertyToSet))
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key was not found on this object. ObjectId = '{0}', Key = '{1}'", Id, propertyKey));
				return;
			}

			if (propertyToSet == null)
				return;

			// Verify the property value being set is an object list
			if (propertyToSet.Type != DevicePropertyType.ObjectList)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property with the provided key is not of type ObjectList. Id = '{0}'", Id));
				return;
			}

			var internalPropertyToSet = (ObjectList)propertyToSet;

			// Verify that the value provided and the value being set are of the same type
			if (internalPropertyToSet.SubType != value.SubType)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format(
						"The list value provided and the property value being set do not contains objects of the same class type. ValueProvidedType = '{0}', ValueBeingSetType = '{1}'",
						value.SubType, internalPropertyToSet.SubType));
				return;
			}


			internalPropertyToSet.ListMembers = value.ListMembers;
			value.PropertyChanged += ValueOnPropertyChanged;

			RaisePropertyChangedEvent();
		}

		private void ValueOnPropertyChanged(object sender, EventArgs eventArgs)
		{
			// If the value of one of the child properties of this object changes also raise the event for this object
			RaisePropertyChangedEvent();
		}

		protected override List<string> GetMembers()
		{
			return ObjectProperties.Values.Select(property => property.Id).ToList();
		}
	}

	/// <summary>
	/// Represents the intance of a <see cref="PropertyValue"/> that is of type <see cref="DevicePropertyType.ObjectList"/>.
	/// </summary>
	public class ObjectList : PropertyValue, IPropertyListValue
	{
		//private readonly CCriticalSection _listLock = new CCriticalSection();
		private readonly object _listLock = new object();

		/// <summary>
		/// Create a property value for an object that is a member of a list.
		/// </summary>
		/// <param name="definitionKey">The property definition key.</param>
		/// <param name="listSubType">The type of objects that the list contains.</param>
		/// <param name="logger">Method for logging.</param>
		internal ObjectList(string definitionKey, string listSubType, Action<string> logger)
			: base(definitionKey, DevicePropertyType.ObjectList, logger)
		{
			SubType = listSubType;
			ListMembers = new List<ObjectValue>();
			ClassNameForLogging = "ObjectList";
		}

		/// <summary>
		/// The class type of the objects that this list contains. See <see cref="ClassDefinition.ClassName"/>.
		/// </summary>
		public string SubType { get; private set; }

		/// <summary>
		/// List containing all of the objects in this <see cref="ObjectList"/>.
		/// </summary>
		internal List<ObjectValue> ListMembers { get; set; }

		/// <summary>
		/// Add the provided object to the list.
		/// </summary>
		/// <param name="objectValueToAdd">
		/// The object to add. The <see cref="PropertyValue.Type"/> of this object must be of type <see cref="DevicePropertyType.Object"/>
		/// and the <see cref="PropertyDefinition.SubType"/> of this object must match the <see cref="PropertyDefinition.SubType"/>
		/// of the list it is being added to.
		/// </param>
		public void AddObject(ObjectValue objectValueToAdd)
		{
			const string methodNameForLogging = "AddObject";

			// Verify the provided object being added is actually an object
			if (objectValueToAdd.Type != DevicePropertyType.Object)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format("The property you are attempted to add is not of type Object. Id = '{0}'", Id));
			}

			// Verify that the object being added subtype matches the list subtype
			if (objectValueToAdd.SubType != SubType)
			{
				ExtensionDeviceLogHelper.LogMessage(Logger, LogLevel.Warning, ClassNameForLogging, methodNameForLogging,
					string.Format(
						"The subtype of the object being added does not match the subtype of the list. ObjectValue SubType = '{0}', List SubType = '{1}'",
						objectValueToAdd.SubType, SubType));
				return;
			}

			// Add the property to the list
			try
			{
				//_listLock.Enter();
				lock (_listLock)
					ListMembers?.Add(objectValueToAdd);
			}
			finally
			{
				//_listLock.Leave();
			}

			objectValueToAdd.ParentIds.Add(Id);
			RaisePropertyChangedEvent();
		}

		/// <summary>
		/// Remove the provided object from the list.
		/// <para>
		/// If the object is not in the list this method will do nothing.
		/// If there are multiple occurences of the object in the list then this method will remove the first occurence.
		/// </para>
		/// </summary>
		/// <param name="objectId">The id of the object to remove.</param>
		public void RemoveObject(string objectId)
		{
			// If the list is empty return
			if (ListMembers == null || ListMembers.Count == 0)
				return;

			// If the list doesn't contain the object return
			if (!ListMembers.Exists(x => x.Id == objectId))
				return;

			// Remove the first occurance of the object from the list
			foreach (var item in ListMembers)
			{
				if (item.Id == objectId)
				{
					try
					{
						//_listLock.Enter();
						lock (_listLock)
							ListMembers?.Remove(item);
					}
					finally
					{
						//_listLock.Leave();
					}

					item.ParentIds.Remove(Id);
				}
				break;
			}

			RaisePropertyChangedEvent();
		}

		/// <summary>
		/// Clears all objects from the provided list.
		/// <para>
		/// If the list is already empty this method will do nothing.
		/// </para>
		/// </summary>
		public void Clear()
		{
			// If the list is empty return
			if (ListMembers == null || ListMembers.Count == 0)
				return;

			try
			{
				//_listLock.Enter();

				lock (_listLock)
				{

					// Remove this list as the parent for each element in the list
					foreach (var item in ListMembers)
						item?.ParentIds?.Remove(Id);

					// Clear the list and queue the event
					ListMembers?.Clear();
				}

			}
			finally
			{
				//_listLock.Leave();
			}

			RaisePropertyChangedEvent();
		}

		protected override List<string> GetMembers()
		{
			return ListMembers.Select(property => property.Id).ToList();
		}
	}
}
