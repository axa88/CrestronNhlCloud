// ReSharper disable once CheckNamespace
namespace Crestron.RAD.Common.Interfaces.ExtensionDevice
{
	/// <summary>
	/// Represents a value that an <see cref="IPropertyValue"/> can be set to.
	/// </summary>
	public interface IPropertyAvailableValue
	{
		/// <summary>
		/// The user visible label of the availbale value. If a translation key is provided then the translated string will be prioritized over the label.
		/// </summary>
		string Label { get; }

		/// <summary>
		/// Key to look up the user visable translation for this value in the translation dictionary.
		/// </summary>
		string LabelLocalizationKey { get; }

		/// <summary>
		/// True if this value can be selected in ui, false if this value can not be selected.
		/// </summary>
		bool Enabled { set; get; }
	}

	/// <summary>
	/// Represents a value that an <see cref="IPropertyValue"/> can be set to.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public interface IPropertyAvailableValue<T> : IPropertyAvailableValue
	{
		/// <summary>
		/// The internal value that the device will receive, does not appear in the UI 
		/// </summary>
		T Value { get; }
	}
}