using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Enums
{
	/// <summary>
	/// Defines the possible types of a device property;
	/// Specified by the interface property <see cref="IDeviceProperty.Type"/>.
	/// </summary>
	public enum DevicePropertyType
	{
		Uninitialized,

		/// <summary>
		/// Represents a value of type <see cref="bool"/>
		/// </summary>
		Boolean,

		/// <summary>
		/// Represents a value of type <see cref="string"/>
		/// </summary>
		String,

		/// <summary>
		/// Represents a value of type <see cref="short"/>
		/// </summary>
		Int16,

		/// <summary>
		/// Represents a value of type <see cref="ushort"/>
		/// </summary>
		UInt16,

		/// <summary>
		/// Represents a value of type <see cref="int"/>
		/// </summary>
		Int32,

		/// <summary>
		/// Represents a value of type <see cref="uint"/>
		/// </summary>
		UInt32,

		/// <summary>
		/// Represents a value of type <see cref="long"/>
		/// </summary>
		Int64,

		/// <summary>
		/// Represents a value of type <see cref="ulong"/>
		/// </summary>
		UInt64,

		/// <summary>
		/// Represents a value of type <see cref="float"/>
		/// </summary>
		Float,

		/// <summary>
		/// Represents a value of type <see cref="double"/>
		/// </summary>
		Double,

		/// <summary>
		/// Indicates the type is a localized string that must be resolved using the <see cref="ILocalizedDevice"/> interface.
		/// <para>
		/// The actual value of this property will be equivalent to <see cref="Int32"/>.
		/// That integer then needs to be looked up in the key-value pairs returned
		/// by <see cref="ILocalizedDevice.GetLocalizedStrings"/>.
		/// </para>
		/// </summary>
		LocalizedString,

		/// <summary>
		/// Represents a value of type <see cref="object"/>
		/// </summary>
		Object,

		/// <summary>
		/// Represents a value of type <see cref="List{T}"/> where T is an <see cref="object"/>
		/// </summary>
		ObjectList
	}
}