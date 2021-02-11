﻿namespace NhlApiShared
{
	public interface ISettings
	{
		void PrintLine(string message);
		bool GetBoolSetting(string key);
		void SetBoolSetting(string key, bool value);
		ushort GetNumberSetting(string key);
		void SetNumberSetting(string key, ushort value);
		string GetStringSetting(string key);
		void SetStringSetting(string key, string value);
	}
}
