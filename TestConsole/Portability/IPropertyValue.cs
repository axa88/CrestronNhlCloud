using System.Collections.ObjectModel;


namespace TestConsole.Portability
{
	public interface IPropertyValue
	{
		string Id { get; }

		string DefinitionKey { get; }

		DevicePropertyType Type { get; }

		bool Enabled { get; }

		bool IsValueAvailable { get; }

		bool IsRootProperty { get; }

		ReadOnlyCollection<string> MemberIds { get; }
	}
}