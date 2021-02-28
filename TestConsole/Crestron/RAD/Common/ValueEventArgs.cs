using System;


namespace Crestron.RAD.Common.Events
{
    /// <summary>
    /// Argument for events that refer to a value.
    /// </summary>
    public sealed class ValueEventArgs<T> : EventArgs
    {
        public ValueEventArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        /// New value being referred to by the event that was raised
        /// </summary>
        public T Value { get; private set; }
    }
}