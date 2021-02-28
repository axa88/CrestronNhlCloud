using System;

using static System.Console;


namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var nhlExt = new TheExtension();
			nhlExt.Initialize();
			nhlExt.PreGameStarted += () => WriteLine($"++++++ {DateTime.Now} PREGAME");
			nhlExt.PuckDropped += () => WriteLine($"++++++ {DateTime.Now} PUCK DROP");
			nhlExt.CriticalGamePlayStarted += () => WriteLine($"++++++ {DateTime.Now} Critical Play");
			nhlExt.TeamGoalScored += () => WriteLine($"++++++ {DateTime.Now} GOAL");
			nhlExt.OpponentGoalScored += () => WriteLine($"++++++ {DateTime.Now} OPP");
			nhlExt.GameEnded += () => WriteLine($"++++++ {DateTime.Now} END");

			nhlExt.StatusChanged += status => WriteLine($"+++ status: {status}");

			while (true)
				nhlExt.ConsoleReader(ReadLine());
		}
	}
}