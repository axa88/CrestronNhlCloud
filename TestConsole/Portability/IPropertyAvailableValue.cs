namespace TestConsole.Portability
{
	public interface IPropertyAvailableValue
	{
		string Label { get; }
		string LabelLocalizationKey { get; }
		bool Enabled { set; get; }
	}
}