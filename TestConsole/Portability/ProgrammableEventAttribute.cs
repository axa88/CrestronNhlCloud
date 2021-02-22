using System;

namespace Crestron.RAD.Common.Attributes.Programming
{
	/// <summary>
	/// Specifies an event is programmable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Event)]
	public class ProgrammableEventAttribute : Attribute
	{
		/// <summary>
		/// Returns the display name of the event.
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgrammableEventAttribute"/> class.
		/// See <see cref="ProgrammableEventAttribute(string)"/> to set the display name of the event.
		/// </summary>
		public ProgrammableEventAttribute()
			: this(string.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgrammableEventAttribute"/> class.
		/// </summary>
		/// <param name="displayName">The display name for the event.</param>
		public ProgrammableEventAttribute(string displayName)
		{
			DisplayName = displayName;
		}
	}
}