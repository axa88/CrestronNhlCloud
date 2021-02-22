using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using NhlApiShared.Api.Models.Schedule;

using static NhlApiShared.Common.UiObjects;

using Team = NhlApiShared.Api.Models.Teams.Team;

#if WINDOWS
using TestConsole;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using TestConsole.Transport;
#endif

#if CRESTRON
using CrestronNhlCloud;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using CrestronNhlCloud.Transport;
#endif


namespace NhlApiShared.Common
{
	public class Logic : IDisposable
	{
		public readonly Dictionary<string, IPropertyValue> Properties = new Dictionary<string, IPropertyValue>(); // encapsulate this?
		public readonly List<IPropertyAvailableValue> TeamList = new List<IPropertyAvailableValue>();

		private readonly NhlExtension _extension;
		private Timer _pollTimer;
		private readonly object _pollLock = new object();
		private List<Team> _teams = new List<Team>();
		private int _currentGame;
		private int _myTeamScore;
		private int _opponentScore;
		private Designation _myLastTeam;
		private byte _lastStatusCode;
		private bool _gameStarted;

		private TimeSpan _pregameOffset = TimeSpan.FromMinutes(30);
		private TimeSpan _puckDropOffset = TimeSpan.FromMinutes(8);
		private TimeSpan _liveDelay = TimeSpan.FromSeconds(45);

		public Logic(object ext)
		{
			if (ext is NhlExtension extension)
			{
				_extension = extension;
				_extension.PrintLine("Logic ctor start"); // cant be moved as it needs the ref set
				_extension.PrintLine("Logic ctor end");
			}
			else
				throw new Exception("Bad Logic");
		}

		public event Action PreGameStarted;
		public event Action PuckDropped;
		public event Action CriticalGamePlayStarted;
		public event Action GameEnded;
		public event Action TeamGoalScored;
		public event Action OpponentGoalScored;

		internal void Initialize()
		{
			_extension.PrintLine("ini ini start");

			// Tile
			Properties[TileStatus] = _extension.CreateProperty<string>(TileStatus, DevicePropertyType.String);
			Properties[TileIcon] = _extension.CreateProperty<string>(TileIcon, DevicePropertyType.String);
			//Properties[TileSecondaryIcon] = _extension.CreateProp<string>(TileSecondaryIcon, DevicePropertyType.String);

			// Team Selector
			Properties[SelectorButtonValueTeam] = _extension.CreateProperty<ushort>(SelectorButtonValueTeam, DevicePropertyType.UInt16, TeamList);

			// initial values
			ChangeProperty(TileStatus, "-");
			ChangeProperty(TileIcon, "icBroadcastRegular");
			//ChangeProperty(TileSecondaryIcon, "icSettings");

			_pollTimer = new Timer(PollTimerCallback, null, TimeSpan.FromMinutes(0), Timeout.InfiniteTimeSpan);

			_extension.PrintLine("ini ini end");
		}

		internal void ChangeProperty<T>(string key, T value)
		{
			((PropertyValue<T>)Properties[key]).Value = value;
			_extension.UpdateUi();
		}

		internal T GetProperty<T>(string key) => ((PropertyValue<T>)Properties[key]).Value;

		internal void PollTimerCallback(object state)
		{
			_extension.PrintLine($"{nameof(PollTimerCallback)} start :: {DateTime.Now.ToShortDateString()} :: {DateTime.Now.ToShortTimeString()}");
			try
			{
				lock (_pollLock)
				{
					_extension.Connect(PollInitializationLogic()); // todo replace with connectivity service

					var teamId = GetProperty<ushort>(SelectorButtonValueTeam);
					var pollTime = teamId != default ? PollBusinessLogic(teamId) : TimeSpan.FromMinutes(1);

					_pollTimer.Change(pollTime, Timeout.InfiniteTimeSpan);
				}
			}
			catch (Exception e) { _extension.CatchPrint(e); }
		}

		private bool PollInitializationLogic()
		{
			if (!TeamList.Any())
			{
				_teams = _extension.HttpTransport.GetTeams();
				if (_teams != null)
					AddTeams(_teams);
				else
					_extension.PrintLine("cant get teams");
			}

			if (TeamList.Any() && GetProperty<ushort>(SelectorButtonValueTeam) == default)
			{
				_extension.PrintLine("restore");
				var recalledTeamId = _extension.GetNumberSetting(nameof(DefaultTeamKey));
				if (recalledTeamId != default)
					ChangeProperty(SelectorButtonValueTeam, recalledTeamId);
			}

			return true;
		}

		private TimeSpan PollBusinessLogic(ushort teamId)
		{
			TimeSpan pollTime;

			var schedule = _extension.HttpTransport.GetTeamSchedule(teamId);
			if (schedule != null)
			{
				if (schedule.TotalGames >= 1)
				{
					var game = schedule.Dates.First().Games.First();
					if (byte.TryParse(game.Status.StatusCode, out var statusCode))
					{
						var (myTeam, opponent) = game.Participants.Away.Team.Id == teamId ? (game.Participants.Away, game.Participants.Home) : (game.Participants.Home, game.Participants.Away);

						string status;
						switch (statusCode)
						{
							case 1: // Preview "Scheduled"
								status = $"Game Time: {game.GameDate:H:mm}";
								var previewPollResolution = TimeSpan.FromMinutes(10);
								var previewRemaining = game.GameDate - _pregameOffset - DateTime.Now;
								pollTime = previewRemaining < previewPollResolution ? previewRemaining : previewPollResolution;
								break;
							case 2: // Preview "Pre-Game"
								status = $"{game.Status.DetailedState}";
								if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
									PreGameStarted?.Invoke();
								pollTime = game.GameDate > DateTime.Now ? TimeSpan.FromMinutes(1) : TimeSpan.FromSeconds(10);
								break;
							case 8: // Preview "Scheduled (Time TBD)"
								status = $"Game Time: TBD";
								pollTime = TimeSpan.FromMinutes(10);
								break;
							case 3: // Live "In Progress"
								var liveStatus = myTeam.Score > opponent.Score ? "W" : myTeam.Score < opponent.Score ? "L" : "";
								status = $"{game.Status.AbstractGameState}: {liveStatus} {game.Participants.Away.Score} - {game.Participants.Home.Score}";
								if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
									PuckDropped?.Invoke();
								pollTime = TimeSpan.FromSeconds(3);
								break;
							case 4: // Live "In Progress - Critical" | last 5 minutes
								var criticalStatus = myTeam.Score > opponent.Score ? "W" : myTeam.Score < opponent.Score ? "L" : "";
								status = $"5 To Go: {criticalStatus} {game.Participants.Away.Score} - {game.Participants.Home.Score}";
								if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
									CriticalGamePlayStarted?.Invoke();
								pollTime = TimeSpan.FromSeconds(1.5);
								break;
							case 5: // Final "Game Over"
							case 6: // Final "Final"
							case 7: // Final "Final"
								var finalStatus = myTeam.Score > opponent.Score ? "W" : myTeam.Score < opponent.Score ? "L" : "";
								status = $"{game.Status.DetailedState}: {finalStatus} {game.Participants.Away.Score} - {game.Participants.Home.Score}";
								if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
									GameEnded?.Invoke();
								pollTime = TimeSpan.FromMinutes(1); // ToDo fix this after eval
								break;
							case 9: // Preview "Postponed"
								status = $"{game.Status.DetailedState}";
								pollTime = TimeSpan.FromMinutes(15);
								break;
							default: throw new InvalidEnumArgumentException($"invalid {nameof(game.Status.DetailedState)}");
						}

						ChangeProperty(TileStatus, status);
						_extension.PrintLine($"{game.Status.AbstractGameState}");
						_lastStatusCode = statusCode;

						// Goal Events
						if (statusCode == 3 || statusCode == 4) // Live; can get hard data, class: GameStatus #3 AbstractGameState and avoid breakage
						{
							// first make sure in sync, system may have just boot or api offline || Team change (H<>A team swap)
							if (_currentGame != game.GamePk || myTeam.Team.Id != _myLastTeam.Team.Id)
							{
								_extension.PrintLine("game update");
								_myLastTeam = myTeam;
								_currentGame = game.GamePk;
								_myTeamScore = myTeam.Score;
								_opponentScore = opponent.Score;
							}

							// check
							if (_myTeamScore != myTeam.Score)
							{
								_myTeamScore = myTeam.Score;
								TeamGoalScored?.Invoke();
							}

							if (_opponentScore != opponent.Score)
							{
								_opponentScore = opponent.Score;
								OpponentGoalScored?.Invoke();
							}
						}
					}
					else // offline condition ToDo implement ping loopback or some connectivity service
					{
						_extension.PrintLine("no game data");
						pollTime = TimeSpan.FromMinutes(10);
					}
				}
				else
				{
					ChangeProperty(TileStatus, "No Game Today");
					pollTime = TimeSpan.FromMinutes(10);
				}
			}
			else // offline condition ToDo implement ping loopback or some connectivity service
			{
				_extension.PrintLine("no schedule");
				pollTime = TimeSpan.FromMinutes(10);
			}

			return pollTime;
		}

		private void AddTeams(IEnumerable<Team> teams)
		{
			foreach (var team in teams)
				TeamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));

			if (TeamList.Any())
				_extension.UpdateUi();
		}

		#region IDisposable
		public void Dispose()
		{
			_extension?.PrintLine("Logic Dispose");
			_pollTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			_pollTimer?.Dispose();
		}
		#endregion
	}
}
