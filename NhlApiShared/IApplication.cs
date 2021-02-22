using System;


namespace NhlApiShared
{
	public interface IApplication
	{
		event Action PreGameStarted;
		event Action PuckDropped;
		event Action CriticalGamePlayStarted;
		event Action GameEnded;
		event Action TeamGoalScored;
		event Action OpponentGoalScored;
	}
}
