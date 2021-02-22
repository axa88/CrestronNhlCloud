using System;
using System.Collections.Generic;
using RADCommon.Interfaces;

namespace Crestron.RAD.Common.BasicDriver
{
    /// <summary>
    /// Dictionary-based implementation of IInterfaceLookup
    /// Recursively adds base implementations to the dictionary, optimizing for lookup
    /// speed instead of dictionary size.
    /// </summary>
    /// <typeparam name="TBaseInterface">The base interface types which entries must derive from. Use object if you want to use all interfaces.</typeparam>
    public class InterfaceLookup<TBaseInterface> : IInterfaceLookup<TBaseInterface> where TBaseInterface : class
    {
        /// <summary>
        /// Dictionary that holds the interface implementations
        /// </summary>
        private readonly IDictionary<Type, TBaseInterface> _implementations;

        /// <summary>
        /// Initializes the underlying dictionary for the lookup
        /// </summary>
        public InterfaceLookup()
        {
            _implementations = new Dictionary<Type, TBaseInterface>();
        }

        /// <summary>
        /// Recursively registers an implementation of an interface to be looked up later via
        /// <see cref="TryGetInterface"/>. If the type passed is not an interface, the type
        /// itself is not added but the interfaces it implements are.
        /// Overwrites any previously registered interfaces of this type or base interfaces.
        /// </summary>
        /// <param name="t">The interface type to register for</param>
        /// <param name="implementation">The object which provides the interface</param>
        public void RegisterInterface(Type t, object implementation)
        {
            TBaseInterface impl = implementation as TBaseInterface;

            // Verify the implementation object is not null and is an instance of TBaseInterface
            if (impl == null)
            {
                if (implementation == null)
                {
                    throw new ArgumentNullException("implementation");
                }
                throw new ArgumentException("Invalid interface", "implementation");
            }

            // If called with a class Type, register its interfaces but not the class itself
            if (t.IsInterface)
            {
                _implementations[t] = impl;
            }

            Type baseInterfaceType = typeof(TBaseInterface);
            foreach (Type subInterface in t.GetInterfaces())
            {
                if (baseInterfaceType.IsAssignableFrom(subInterface) && baseInterfaceType != subInterface)
                {
                    _implementations[subInterface] = impl;
                }
            }
        }

        /// <summary>
        /// Get a previously registered object which implements T, or null if none is found.
        /// </summary>
        /// <param name="t">The interface type to retrieve an implementation of</param>
        /// <returns>An object implementing T, or null if none was found</returns>
        public object TryGetInterface(Type t)
        {
            TBaseInterface result;
            _implementations.TryGetValue(t, out result);
            return result;
        }
    }
}