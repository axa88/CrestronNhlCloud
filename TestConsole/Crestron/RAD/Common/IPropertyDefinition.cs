using System.Collections.Generic;
using Crestron.RAD.Common.Enums;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Interfaces.ExtensionDevice
{
	/// <summary>
	/// The definition of a property on an <see cref="IExtensionDevice"/>.
	/// <para>
	/// To get the value of this property see <see cref="IPropertyValue"/>.
	/// </para>
	/// </summary>
	public interface IPropertyDefinition
	{
		/// <summary>
		/// Identification key for this property.
		/// </summary>
		string Key { get; }

		/// <summary>
		/// Key used for looking up the property name in the translation dictionary.
		/// </summary>
		string NameLocalizationKey { get; }

		/// <summary>
		/// Indictates the type of the property.
		/// </summary>
		DevicePropertyType Type { get; }

		/// <summary>
		/// This is only used for properties of Type <see cref="DevicePropertyType.Object"/> or <see cref="DevicePropertyType.ObjectList"/>.
		/// <para>
		/// For an <see cref="DevicePropertyType.Object"/>, this is the type that the object is. See <see cref="IClassDefinition.ClassName"/>.
		/// </para>
		/// <para>
		/// For an <see cref="DevicePropertyType.ObjectList"/>, this is the type the the list contains. See <see cref="IClassDefinition.ClassName"/>.
		/// </para>
		/// </summary>
		string SubType { get; }

		/// <summary>
		/// Stores the type of the object that this property belongs too. See <see cref="IClassDefinition.ClassName"/>.
		/// </summary>
		string AssociatedClass { get; }

		/// <summary>
		/// If not null, this is the list of values that this property can be set to.
		/// </summary>
		IEnumerable<IPropertyAvailableValue> AvailableValues { get; }

		/// <summary>
		/// The minimum value that this property can be set to.
		/// <para>
		/// For numeric types, this is the minimum numeric value, and for strings, this is the minimum length of the string.
		/// </para>
		/// </summary>
		double? MinValue { get; }

		/// <summary>
		/// The maximum value that this property can be set to.
		/// <para>
		/// For numeric types, this is the maximum numeric value, and for strings, this is the maximum length of the string.
		/// </para>
		/// </summary>
		double? MaxValue { get; }

		/// <summary>
		/// The increment size that this property can be set to starting at <see cref="MinValue"/> and ending at <see cref="MaxValue"/>.
		/// <para>
		/// This is only valid for numeric types.
		/// </para>
		/// </summary>
		double? StepSize { get; }
	}
}