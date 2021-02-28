using System;

using Crestron.SimplSharp;

using NhlApiShared.Common;


namespace CrestronNhlCloud
{
	public class TheExtension : Extension
	{
		public TheExtension()
		{
			try
			{
				Log("RAD EX: cstr start");

				Log("RAD EX: cstr end");
			}
			catch (Exception e)
			{
				CrestronConsole.PrintLine(e.Message);
				CrestronConsole.PrintLine(e.StackTrace);
			}
		}
	}
}
