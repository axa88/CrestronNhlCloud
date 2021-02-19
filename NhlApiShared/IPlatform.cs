using System.Collections.Generic;

#if WINDOWS
using TestConsole;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using TestConsole.Transport;
#endif

#if CRESTRON
using CrestronNhlCloud;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using CrestronNhlCloud.Transport;
#endif


namespace NhlApiShared
{
	public interface IPlatform
	{
		// Generic
		void Connect(bool state);
		void PrintLine(string message);

		// Ui
		PropertyValue<T> CreateProperty<T>(string key, DevicePropertyType type, IEnumerable<IPropertyAvailableValue> availableValues = null);
		void UpdateUi();
	}
}
