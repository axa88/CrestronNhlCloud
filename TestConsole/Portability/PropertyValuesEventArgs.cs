using System;
using System.Collections.Generic;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;

// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Events
{
	/// <summary>
	/// Event args containing all of the property values that have changed since the last event.
	/// </summary>
	public class PropertyValuesEventArgs : EventArgs
	{
		/// <summary>
		/// Create a new property values changed event args.
		/// </summary>
		public PropertyValuesEventArgs(IEnumerable<IPropertyValue> propertyValues)
		{
			PropertyValues = propertyValues;
		}

		/// <summary>
		/// Contains all of the property values that have changed.
		/// </summary>
		public IEnumerable<IPropertyValue> PropertyValues { get; private set; }
	}
}