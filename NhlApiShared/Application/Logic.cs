using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Api.Models.Teams;

#if WINDOWS
using TestConsole;
#endif

#if CRESTRON
using CrestronNhlCloud;
#endif



using static NhlApiShared.Application.UiObjects;


namespace CrestronNhlCloud.NhlApiShared.Application
{
	public class Logic
	{
		private readonly NhlExtension _extension;
		private readonly Timer _pollTimer;
		private List<Team> _teams = new List<Team>();
		private int _currentGame;
		private int _score;


		public Logic(object ex)
		{
			if (ex is NhlExtension extension)
			{
				extension.PrintLine("Logic ctor start");
				_extension = extension;
				_pollTimer = new Timer(PollTimerCallback, null, TimeSpan.FromMinutes(0), Timeout.InfiniteTimeSpan);
				extension.PrintLine("Logic ctor end");
			}
		}

		public event Action GoalScored;

		public void PollTimerCallback(object state)
		{
			//if (!(state is NhlExtension extension))
			//{
			//	_extension?.PrintLine("Logic poll obj bad");
			//	return;
			//}

			try
			{
				{ // uninitialized
					if (!_extension.TeamList.Any())
					{
						_extension.PrintLine("initialize");
						_teams = _extension.HttpTransport.GetTeams();
						if (_teams == null)
						{
							_extension.PrintLine("cant connect to nhl");
							_pollTimer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
						}
						else
						{
							_extension.AddTeams(_teams);
							if (_extension.TeamList.Any())
							{
								_extension.UpdateUi();
								_extension.Connect(true);
								_extension.PrintLine("Teams added, connected");
							}
						}
					}
				}

				{ // restore default
					if (_extension.GetProperty<ushort>(SelectorButtonValueTeam) == default && _extension.TeamList.Any())
					{
						_extension.PrintLine("restore");

						var teamId = _extension.GetNumberSetting(nameof(DefaultTeamKey));
						_extension.PrintLine($"def teamId: {teamId}");

						if (teamId != default)
						{
							_extension.PrintLine("check");
							_extension.SetProperty(SelectorButtonValueTeam, teamId);
							_extension.UpdateUi();
							_extension.PrintLine("committed");
						}
						else
						{
							_extension.PrintLine("no def");
						}

						_extension.PrintLine("done");
					}
				}

				#region function

				{ // poll
					var teamId = _extension.GetProperty<ushort>(SelectorButtonValueTeam);
					if (teamId != default)
					{
						_extension.PrintLine("Poll");

						var schedule = _extension.HttpTransport.GetTeamSchedule(teamId);
						if (schedule == null)
						{
							_extension.PrintLine("no sched");
							_extension.Connect(false);
							_pollTimer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
						}
						else
						{
							if (schedule.TotalGames < 1)
							{
								_extension.PrintLine("no game");
								_extension.SetProperty(TileStatus, "No Game Today");
								_extension.UpdateUi();
								// schedule next check at midnite
							}
							else
							{
								_extension.PrintLine("game on");

								#region statuses
								// todo account for more than one gate in a day.
								/*"status" : {
								"abstractGameState" : "Preview",
								"codedGameState" : "1",
								"detailedState" : "Scheduled",
								"statusCode" : "1",
								"startTimeTBD" : false
							}*/

								/*"status": {
								"abstractGameState": "Final",
								"codedGameState": "7",
								"detailedState": "Final",
								"statusCode": "7",
								"startTimeTBD": true
							}*/
								/*"status" : {
								"abstractGameState" : "Preview",
								"codedGameState" : "9",
								"detailedState" : "Postponed",
								"statusCode" : "9",
								"startTimeTBD" : false
							}*/
								#endregion

								var game = schedule.Dates.First().Games.First();
								var team = game.Teams.Away.Team.Id == teamId ? game.Teams.Away : game.Teams.Home;

								_extension.SetProperty(TileStatus, $"Game Time:{Environment.NewLine}{game.GameDate:hh:mm t}");
								_extension.UpdateUi();

								// BUG changing teams in same game causes false goal. + others

								// first make sure in sync, system may have just booted or api offline
								if (_currentGame != game.GamePk)
								{
									_extension.PrintLine("trackers reset");
									_currentGame = game.GamePk;
									_score = team.Score;
								}

								if (_score != team.Score)
								{
									_extension.PrintLine(">>>>>>>>>>>>>> GOAL !!!!!!!!!!!!!!!");
									_extension.SetProperty(TileStatus, $"GOAL!{Environment.NewLine}{game.Teams.Away.Score}-{game.Teams.Home.Score}");
									GoalScored?.Invoke();
								}

								// adjust poll timing
								if (ushort.TryParse(game.Status.StatusCode, out var statusCode) && statusCode > 6) // game over
								{
									// schedule next check at midnite
								}
								else if (game.GameDate - DateTime.Now > TimeSpan.FromMinutes(10))
								{
									_extension.PrintLine("time > 10");
									_pollTimer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
								}
								else if (game.GameDate - DateTime.Now > TimeSpan.FromMinutes(1))
								{
									_extension.PrintLine("time > 1");
									_pollTimer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
								}
								else
								{
									_extension.PrintLine("else poll");
									_pollTimer.Change(TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
								}
							}
						}
					}
				}

				#endregion function
			}
			catch (Exception exception)
			{
				_extension.PrintLine(nameof(PollTimerCallback));
				_extension.PrintLine(exception.Message);
			}
		}
	}
}
