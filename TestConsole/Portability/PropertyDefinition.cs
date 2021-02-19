using System;
using System.Collections.Generic;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.DeviceTypes.ExtensionDevice
{
	/// <summary>
	/// The definition of a property on an <see cref="IExtensionDevice"/>.
	/// <para>
	/// To get the value of this property see <see cref="IPropertyValue"/>.
	/// </para>
	/// </summary>
	public class PropertyDefinition : IPropertyDefinition
	{
		private IEnumerable<IPropertyAvailableValue> _availableValues;
		private double? _minValue;
		private double? _maxValue;
		private double? _stepSize;

		/// <summary>
		/// Create a simple property definition.
		/// </summary>
		/// <param name="key">The key of property.</param>
		/// <param name="nameLocalizationKey">The translation key.</param>
		/// <param name="type">The type of the property.</param>
		public PropertyDefinition(string key, string nameLocalizationKey, DevicePropertyType type)
			: this(key, nameLocalizationKey, type, null, null, null, null, null)
		{
		}

		/// <summary>
		/// Create a property definition for a property of type <see cref="DevicePropertyType.Object"/> or <see cref="DevicePropertyType.ObjectList"/>.
		/// </summary>
		/// <param name="key">The key of property.</param>
		/// <param name="nameLocalizationKey">The translation key.</param>
		/// <param name="type">
		/// The type of the property. In this case it should be <see cref="DevicePropertyType.Object"/> or <see cref="DevicePropertyType.ObjectList"/>.
		/// </param>
		/// <param name="classDefinition">
		/// For <see cref="DevicePropertyType.Object"/> this is the class definition of the object.
		/// <para>
		/// For <see cref="DevicePropertyType.ObjectList"/> this is the class definition of the objects that the list contains.
		/// </para>
		/// </param>
		public PropertyDefinition(string key, string nameLocalizationKey, DevicePropertyType type, IClassDefinition classDefinition)
			: this(key, nameLocalizationKey, type, classDefinition, null, null, null, null)
		{
		}

		/// <summary>
		/// Create a property definition for a property with a set amount of available values.
		/// </summary>
		/// <param name="key">The key of property.</param>
		/// <param name="nameLocalizationKey">The translation key.</param>
		/// <param name="type">The type of the property.</param>
		/// <param name="availableValues">The values that this property can be set to.</param>
		public PropertyDefinition(string key, string nameLocalizationKey, DevicePropertyType type, IEnumerable<IPropertyAvailableValue> availableValues)
			: this(key, nameLocalizationKey, type, null, availableValues, null, null, null)
		{
		}

		/// <summary>
		/// Create a property definition for a property with a minimum value, maximum value, and step stize.
		/// </summary>
		/// <param name="key">The key of property.</param>
		/// <param name="nameLocalizationKey">The translation key.</param>
		/// <param name="type">The type of the property.</param>
		/// <param name="minValue">The minimum value that this property can be set to.</param>
		/// <param name="maxValue">The maximum value that this property can be set to.</param>
		/// <param name="stepSize">The increment size that this property can be set to starting at <see cref="MinValue"/> and ending at <see cref="MaxValue"/>.</param>
		public PropertyDefinition(string key, string nameLocalizationKey, DevicePropertyType type, double minValue, double maxValue, double stepSize)
			: this(key, nameLocalizationKey, type, null, null, minValue, maxValue, stepSize)
		{
		}

		private PropertyDefinition(
			string key,
			string nameLocalizationKey,
			DevicePropertyType type,
			IClassDefinition classDefinition,
			IEnumerable<IPropertyAvailableValue> availableValues,
			double? minValue,
			double? maxValue,
			double? stepSize)
		{
			Key = key;
			NameLocalizationKey = nameLocalizationKey;
			Type = type;
			AvailableValues = availableValues;
			MinValue = minValue;
			MaxValue = maxValue;
			StepSize = stepSize;

			if (classDefinition != null)
				SubType = classDefinition.ClassName;
		}

		/// <summary>
		/// Identification key for this property.
		/// </summary>
		public string Key { get; private set; }

		/// <summary>
		/// Key used for looking up the property name in the translation dictionary.
		/// </summary>
		public string NameLocalizationKey { get; private set; }

		/// <summary>
		/// Indictates the type of the property.
		/// </summary>
		public DevicePropertyType Type { get; private set; }

		/// <summary>
		/// This is only used for properties of Type <see cref="DevicePropertyType.Object"/> or <see cref="DevicePropertyType.ObjectList"/>.
		/// <para>
		/// For an <see cref="DevicePropertyType.Object"/>, this is the type that the object is. See <see cref="IClassDefinition.ClassName"/>.
		/// </para>
		/// <para>
		/// For an <see cref="DevicePropertyType.ObjectList"/>, this is the type the the list contains. See <see cref="IClassDefinition.ClassName"/>.
		/// </para>
		/// </summary>
		public string SubType { get; private set; }

		/// <summary>
		/// Stores the type of the object that this property belongs too. See <see cref="IClassDefinition.ClassName"/>.
		/// </summary>
		public string AssociatedClass { get; internal set; }

		/// <summary>
		/// If not null, this is the list of values that this property can be set to.
		/// </summary>
		public IEnumerable<IPropertyAvailableValue> AvailableValues
		{
			get { return _availableValues; }
			set
			{
				_availableValues = value;
				RaisePropertyDefinitionChangedEvent();
			}
		}

		/// <summary>
		/// The minimum value that this property can be set to.
		/// <para>
		/// For numeric types, this is the minimum numeric value, and for strings, this is the minimum length of the string.
		/// </para>
		/// </summary>
		public double? MinValue
		{
			get { return _minValue; }
			set
			{
				if (_minValue == value)
					return;

				_minValue = value;
				RaisePropertyDefinitionChangedEvent();
			}
		}

		/// <summary>
		/// The maximum value that this property can be set to.
		/// <para>
		/// For numeric types, this is the maximum numeric value, and for strings, this is the maximum length of the string.
		/// </para>
		/// </summary>
		public double? MaxValue
		{
			get { return _maxValue; }
			set
			{
				if (_maxValue == value)
					return;

				_maxValue = value;
				RaisePropertyDefinitionChangedEvent();
			}
		}

		/// <summary>
		/// The increment size that this property can be set to starting at <see cref="MinValue"/> and ending at <see cref="MaxValue"/>.
		/// <para>
		/// This is only valid for numeric types.
		/// </para>
		/// </summary>
		public double? StepSize
		{
			get { return _stepSize; }
			set
			{
				if (_stepSize == value)
					return;

				_stepSize = value;
				RaisePropertyDefinitionChangedEvent();
			}
		}

		internal event EventHandler DefinitionChanged;

		private void RaisePropertyDefinitionChangedEvent()
		{
			if (DefinitionChanged == null)
				return;

			DefinitionChanged.Invoke(this, EventArgs.Empty);
		}
	}
}