using System;

namespace RADCommon.Interfaces
{
    // ReSharper disable once UnusedTypeParameter
	/// <summary>
	/// Describes an object which can look up an implementation of an interface
	/// and can have implementations registered for those interfaces.
	/// It supports "recursively" adding an implementation so that the base interfaces
	/// implemented by that interface are also retrievable from the lookup.
	/// </summary>
	/// <typeparam name="TBaseInterface">The base interface types which entries must derive from. Use object if you want to use all interfaces.</typeparam>
	public interface IInterfaceLookup<TBaseInterface> where TBaseInterface : class
	{
		/// <summary>
		/// Recursively registers an implementation of an interface to be looked up later via
		/// <see cref="TryGetInterface"/>. If the type passed is not an interface, the type
		/// itself is not added but the interfaces it implements are.
		/// Overwrites any previously registered interfaces of this type or base interfaces.
		/// </summary>
		/// <param name="t">The interface type to register for</param>
		/// <param name="implementation">The object which provides the interface</param>
		void RegisterInterface(Type t, object implementation);

		/// <summary>
		/// Get a previously registered object which implements T, or null if none is found.
		/// </summary>
		/// <param name="t">The interface type to retrieve an implementation of</param>
		/// <returns>An object implementing T, or null if none was found</returns>
		object TryGetInterface(Type t);
	}
}