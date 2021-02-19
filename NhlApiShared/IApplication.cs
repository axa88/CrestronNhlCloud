using System;
using System.Collections.Generic;

using NhlApiShared.Api.Models.Teams;


namespace NhlApiShared
{
	public interface IApplication
	{
		event Action PreGameStarted;
		event Action GameStarted;
		event Action PuckDropped;
		event Action OverTimeStarted;
		event Action GameEnded;
		event Action TeamGoalScored;
		event Action OpponentGoalScored;

		//IEnumerable<object> TeamList { get; }

		//void AddTeams(IEnumerable<Team> teams);

		//void TeamChanged();
	}
}
