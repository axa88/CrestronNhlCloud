namespace NhlApiShared
{
	public interface IPlatform
	{
		// Ui
		T GetProperty<T>(string key);
		void SetProperty<T>(string key, T value);
		void UpdateUi();

		// Generic
		void Connect(bool state);
	}
}
