using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace TestConsole.Portability
{
	public class PropertyValue<T> : IPropertyValue
	{
		private T _value;
		
		private bool _enabled;
		private bool _isValueAvailable;
		protected readonly Action<string> Logger;
		protected string ClassNameForLogging;
		private EventHandler _eventHandler;

		protected PropertyValue(string definitionKey, DevicePropertyType type, Action<string> logger)
		{
			DefinitionKey = definitionKey;
			Type = type;
			Logger = logger;
			_enabled = true;
			_isValueAvailable = true;
		}

		internal event EventHandler PropertyChanged;

		public string Id { get; set; } = Guid.NewGuid().ToString();

		public string DefinitionKey {get; }

		public DevicePropertyType Type { get; }

		public bool Enabled
		{
			get => _enabled;
			set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				RaisePropertyChangedEvent();
			}
		}

		public bool IsValueAvailable
		{
			get => _isValueAvailable;
			set
			{
				if (_isValueAvailable == value)
					return;

				_isValueAvailable = value;
				RaisePropertyChangedEvent();
			}
		}

		bool IPropertyValue.IsRootProperty => ParentIds.Count == 0;

		ReadOnlyCollection<string> IPropertyValue.MemberIds => new ReadOnlyCollection<string>(GetMembers());
		internal List<string> ParentIds { get; } =  new List<string>();

		protected virtual List<string> GetMembers() => new List<string>();
		protected void RaisePropertyChangedEvent() => _eventHandler?.Invoke(this, EventArgs.Empty);

		public T Value
		{
			get => _value;
			set
			{
				if (EqualityComparer<T>.Default.Equals(_value, value))
					return;

				_value = value;
				RaisePropertyChangedEvent();
			}
		}
	}
}
