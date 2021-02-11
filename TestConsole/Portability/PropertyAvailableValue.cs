using System;


namespace TestConsole.Portability
{
	public class PropertyAvailableValue<T> : IPropertyAvailableValue
	{
		public PropertyAvailableValue(T value, DevicePropertyType type, string labelLocalizationKey, string label)
		{
			// this is most certainly wrong
			if (typeof(T) != typeof(Type))
				throw new ArgumentException("The provided value type T does not match the provided DevicePropertyType");

			Value = value;

			LabelLocalizationKey = labelLocalizationKey;
			Label = label;
			Enabled = true;
		}

		public T Value { get; set; }

		#region Implementation of IPropertyAvailableValue
		public string Label { get; }
		public string LabelLocalizationKey { get; }
		public bool Enabled { get; set; }
		#endregion
	}
}