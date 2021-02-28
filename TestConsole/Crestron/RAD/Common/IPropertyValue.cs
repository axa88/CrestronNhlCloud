using System.Collections.ObjectModel;
using Crestron.RAD.Common.Enums;


// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Interfaces.ExtensionDevice
{
	/// <summary>
	/// The value of an <see cref="IPropertyDefinition"/>.
	/// </summary>
	public interface IPropertyValue
	{
		/// <summary>
		/// A unique identifier to a specific instance of a property value.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Indicates the definition of this property value. See <see cref="IPropertyDefinition.Key"/>.
		/// </summary>
		string DefinitionKey { get; }

		/// <summary>
		/// Represents the type of this property value. The type is based on the <see cref="IPropertyDefinition.Type"/>.
		/// </summary>
		DevicePropertyType Type { get; }

		/// <summary>
		/// Indicates if the property is currently enabled. If false, the value should not be selectable or editable by the user.
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// Indicates if the property Value is currently available. If false, the current value of <see cref="IPropertyValue{T}.Value"/> should be ignored.
		/// </summary>
		bool IsValueAvailable { get; }

		/// <summary>
		/// True if this property is a root property, meaning that it does not have a parent.
		/// </summary>
		bool IsRootProperty { get; }

		/// <summary>
		/// Contains the Id's of the children to this property.
		/// </summary>
		ReadOnlyCollection<string> MemberIds { get; }
	}

	/// <summary>
	/// The value of a basic property on an <see cref="IExtensionDevice"/> (int, string, bool, etc.).
	/// </summary>
	/// <typeparam name="T">The type of the property</typeparam>
	public interface IPropertyValue<T> : IPropertyValue
	{
		/// <summary>
		/// The current value of the property.
		/// </summary>
		T Value { get; }
	}

	/// <summary>
	/// The value of an object property on an <see cref="IExtensionDevice"/>.
	/// </summary>
	public interface IPropertyObjectValue : IPropertyValue
	{
		/// <summary>
		/// The name of the class used to represent this property. See <see cref="IClassDefinition.ClassName"/>
		/// </summary>
		string SubType { get; }
	}

	/// <summary>
	/// The value of a list property on an <see cref="IExtensionDevice"/>.
	/// </summary>
	public interface IPropertyListValue : IPropertyValue
	{
		/// <summary>
		/// The name of the class that this list contains. See <see cref="IClassDefinition.ClassName"/>
		/// </summary>
		string SubType { get; }
	}
}