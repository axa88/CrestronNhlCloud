using System.Collections.ObjectModel;

//using Crestron.SimplSharp;

// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Interfaces.ExtensionDevice
{
	/// <summary>
	/// Defines a class for an extension device.
	/// </summary>
	public interface IClassDefinition
	{
		/// <summary>
		/// The name of the class.
		/// </summary>
		string ClassName { get; }

		/// <summary>
		/// The properties that this object contains by <see cref="IPropertyDefinition.Key"/>.
		/// </summary>
		ReadOnlyDictionary<string, IPropertyDefinition> Properties { get; }
	}
}