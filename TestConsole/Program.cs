using static System.Console;


namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var nhlExtension = new NhlExtension();
			nhlExtension.Initialize();
			nhlExtension.TeamGoalScored += () => WriteLine("+++ GOAL");
			nhlExtension.StatusChanged += status => WriteLine($"+++ status: {status}");

			ReadLine();
		}
	}
}