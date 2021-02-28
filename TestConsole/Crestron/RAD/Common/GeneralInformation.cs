using System;
using System.Collections.Generic;


namespace Crestron.RAD.Common
{
	public class GeneralInformation
	{
		public string DeviceType { get; set; }
		public string Manufacturer { get; set; }
		public string BaseModel { get; set; }
		public DateTime VersionDate { get; set; }
		public string DriverVersion { get; set; }
		public string SdkVersion { get; set; }
		public string Description { get; set; }
		public Guid Guid { get; set; }
		public List<string> SupportedSeries { get; set; }
		public List<string> SupportedModels { get; set; }
		public ExtensionDeviceData ExtensionDeviceData { get; set; }

		public GeneralInformation()
		{
			DeviceType = string.Empty;
			Manufacturer = string.Empty;
			BaseModel = string.Empty;
			VersionDate = new DateTime();
			DriverVersion = string.Empty;
			SdkVersion = string.Empty;
			Description = string.Empty;
			Guid = new Guid();
			SupportedSeries = new List<string>();
			SupportedModels = new List<string>();
			ExtensionDeviceData = new ExtensionDeviceData();
		}
	}
}
