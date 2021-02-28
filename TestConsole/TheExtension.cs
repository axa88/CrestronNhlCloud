using System;

using NhlApiShared.Common;


namespace TestConsole
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
				Log(e.Message);
				Log(e.StackTrace);
			}
		}

		internal event Action<string> StatusChanged;
	}
}
