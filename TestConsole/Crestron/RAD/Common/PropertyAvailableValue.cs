using System;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.DeviceTypes.ExtensionDevice
{
	/// <summary>
	/// Represents a value that an <see cref="IPropertyValue"/> can be set to.
	/// </summary>
	public class PropertyAvailableValue : IPropertyAvailableValue
	{
		/// <summary>
		/// Create a new available value for a property.
		/// </summary>
		protected PropertyAvailableValue(string labelLocalizationKey, string label)
		{
			LabelLocalizationKey = labelLocalizationKey;
			Label = label;
			Enabled = true;
		}

		public string Label { get; private set; }

		public string LabelLocalizationKey { get; private set; }

		public bool Enabled { get; set; }
	}

	/// <summary>
	/// Represents a value that an <see cref="IPropertyValue"/> can be set to.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public class PropertyAvailableValue<T> : PropertyAvailableValue, IPropertyAvailableValue<T>
	{
		/// <summary>
		/// Create a new available value for a property.
		/// </summary>
		/// <param name="value">The actual value. The type of this object must match the provided <see cref="type"/></param>
		/// <param name="type">The type of the value.</param>
		/// <param name="labelLocalizationKey">The translation key. Set to Null to use the <see cref="label"/> parameter and bypass translations.</param>
		/// <param name="label">
		/// The user visable label of the availbale value. If a translation key is provided then the translated string will be prioritized over the label.
		/// </param>
		public PropertyAvailableValue(T value, DevicePropertyType type, string labelLocalizationKey, string label)
			: base(labelLocalizationKey, label)
		{
			if (!ExtensionDevicePropertyHelper.VerifyValueType<T>(type))
				throw new ArgumentException("The provided value type T does not match the provided DevicePropertyType");

			Value = value;
		}

		/// <summary>
		/// The internal value that the device will receive, does not appear in the UI
		/// </summary>
		public T Value { get; private set; }
	}
}