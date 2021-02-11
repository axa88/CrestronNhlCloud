using System.Collections.Generic;

using Api.Models.Teams;


namespace NhlApiShared
{
	public interface IApplication
	{
		IEnumerable<object> TeamList { get; }
		void AddTeams(IEnumerable<Team> teams);
	}
}
