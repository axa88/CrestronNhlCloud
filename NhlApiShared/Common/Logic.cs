using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using NhlApiShared.Api.Models.Schedule;

using static System.TimeSpan;

using static NhlApiShared.Common.UiObjects;

using Team = NhlApiShared.Api.Models.Teams.Team;

#if WINDOWS
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using TestConsole;
using TestConsole.Transport;
#endif

#if CRESTRON
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using CrestronNhlCloud;
using CrestronNhlCloud.Transport;
#endif


namespace NhlApiShared.Common
{
	public class Logic :  IDisposable
	{
		private readonly Dictionary<string, IPropertyValue> _properties = new Dictionary<string, IPropertyValue>(); // encapsulate this?
		private readonly List<IPropertyAvailableValue> _teamList = new List<IPropertyAvailableValue>();

		private readonly NhlExtension _extension;
		private HttpTransport _httpTransport;

		private Timer _pollTimer;
		private readonly object _pollLock = new object();
		private List<Team> _teams = new List<Team>();
		private int _currentGame;
		private int _myTeamScore;
		private int _opponentScore;
		private Designation _myLastTeam;
		private byte _lastStatusCode;

		private readonly TimeSpan _pregameOffset = FromMinutes(30);
		private Timer _goalDelayTimer;
		private Timer _opponentDelayTimer;

		public Logic(object ext)
		{
			if (ext is NhlExtension extension)
			{
				_extension = extension;
				_extension.PrintLine("Logic ctor start"); // cant be moved as it needs the ref set
				_extension.EnableLogging = true;
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
			if (_extension == null)
				return;

			_extension.PrintLine("ini ini start");

			// Create
				// Tile
			_properties[TileStatus] = _extension.CreateProperty<string>(TileStatus, DevicePropertyType.String);
			_properties[TileIcon] = _extension.CreateProperty<string>(TileIcon, DevicePropertyType.String);
			//Properties[TileSecondaryIcon] = _extension.CreateProp<string>(TileSecondaryIcon, DevicePropertyType.String);

				// Team Selector
			_properties[SelectorButtonValueTeamKey] = _extension.CreateProperty<ushort>(SelectorButtonValueTeamKey, DevicePropertyType.UInt16, _teamList);

				// Goal Delay
					// Label
			_properties[ToggleSliderLabelGoalDelayKey] = _extension.CreateProperty<string>(ToggleSliderLabelGoalDelayKey, DevicePropertyType.String);
					// Slider
			_properties[ToggleSliderValueGoalDelayKey] = _extension.CreateProperty<ushort>(ToggleSliderValueGoalDelayKey, DevicePropertyType.UInt16, 0, 60, 1);

			// initial values
			SetProperty(TileStatus, "-");
			SetProperty(TileIcon, "icBroadcastRegular");
			//ChangeProperty(TileSecondaryIcon, "icSettings");
			SetProperty(ToggleSliderValueGoalDelayKey, _extension.GetNumberSetting(ToggleSliderValueGoalDelayKey));
			SetProperty(ToggleSliderLabelGoalDelayKey, GetProperty<ushort>(ToggleSliderValueGoalDelayKey).ToString());

			// assign event handlers
			PreGameStarted += _extension.OnPreGameStarted;
			PuckDropped += _extension.OnOnPuckDropped;
			CriticalGamePlayStarted += _extension.OnCriticalGamePlayStarted;
			GameEnded += _extension.OnGameEnded;
			TeamGoalScored += _extension.OnTeamGoalScored;
			OpponentGoalScored += _extension.OnOpponentGoalScored;

			_httpTransport = new HttpTransport();

			_pollTimer = new Timer(PollTimerCallback, null, FromMinutes(0), Timeout.InfiniteTimeSpan);
			_goalDelayTimer = new Timer(state => { TeamGoalScored?.Invoke(); }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			_opponentDelayTimer = new Timer(state => { OpponentGoalScored?.Invoke(); }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

			_extension.PrintLine("ini ini end");
		}

		private T GetProperty<T>(string key) => ((PropertyValue<T>)_properties[key]).Value;

		private void SetProperty<T>(string key, T value)
		{
			((PropertyValue<T>)_properties[key]).Value = value;
			_extension.UpdateUi();
		}

		private void PollTimerCallback(object state)
		{
			_extension.PrintLine($"{nameof(PollTimerCallback)} start :: {DateTime.Now.ToShortDateString()} :: {DateTime.Now.ToShortTimeString()}");
			try
			{
				lock (_pollLock)
				{
					_extension.Connect(PollInitializationLogic()); // todo replace with connectivity service

					var teamId = GetProperty<ushort>(SelectorButtonValueTeamKey);
					var pollTime = teamId != default ? PollBusinessLogic(teamId) : FromMinutes(1);

					_pollTimer.Change(pollTime, Timeout.InfiniteTimeSpan);
				}
			}
			catch (Exception e) { CatchPrint(e); }
		}

		private bool PollInitializationLogic()
		{
			if (!_teamList.Any())
			{
				_teams = _httpTransport.GetTeams();
				if (_teams != null)
					AddTeams(_teams);
				else
					_extension.PrintLine("cant get teams");
			}

			if (_teamList.Any() && GetProperty<ushort>(SelectorButtonValueTeamKey) == default)
			{
				_extension.PrintLine("restore");
				var recalledTeamId = _extension.GetNumberSetting(nameof(SelectorButtonValueTeamKey));
				if (recalledTeamId != default)
					SetProperty(SelectorButtonValueTeamKey, recalledTeamId);
			}

			return true;
		}

		private TimeSpan PollBusinessLogic(ushort teamId)
		{
			TimeSpan pollTime;

			var schedule = _httpTransport.GetTeamSchedule(teamId);
			if (schedule != null)
			{
				if (schedule.TotalGames >= 1)
				{
					var game = schedule.Dates.First().Games.First();
					if (byte.TryParse(game.Status.StatusCode, out var statusCode))
					{
						var (myTeam, opponent) = game.Participants.Away.Team.Id == teamId ? (game.Participants.Away, game.Participants.Home) : (game.Participants.Home, game.Participants.Away);

						pollTime = StatusCheck(statusCode, game, myTeam, opponent);

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
								_goalDelayTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
								_opponentDelayTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
							}

							// check
							if (_myTeamScore != myTeam.Score)
							{
								_myTeamScore = myTeam.Score;
								_goalDelayTimer.Change(FromSeconds(GetProperty<ushort>(ToggleSliderValueGoalDelayKey)), Timeout.InfiniteTimeSpan);
							}

							if (_opponentScore != opponent.Score)
							{
								_opponentScore = opponent.Score;
								_opponentDelayTimer.Change(FromSeconds(GetProperty<ushort>(ToggleSliderValueGoalDelayKey)), Timeout.InfiniteTimeSpan);
							}
						}
					}
					else // offline condition ToDo implement ping loopback or some connectivity service
					{
						_extension.PrintLine("no game data");
						pollTime = FromMinutes(10);
					}
				}
				else
				{
					SetProperty(TileStatus, "No Game Today");
					pollTime = FromMinutes(10);
				}
			}
			else // offline condition ToDo implement ping loopback or some connectivity service
			{
				_extension.PrintLine("no schedule");
				pollTime = FromMinutes(10);
			}

			return pollTime;
		}

		private void AddTeams(IEnumerable<Team> teams)
		{
			foreach (var team in teams)
				_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));

			if (_teamList.Any())
				_extension.UpdateUi();
		}

		private TimeSpan StatusCheck(byte statusCode, Game game, Designation myTeam, Designation opponent)
		{
			TimeSpan pollTime;
			string status;
			switch (statusCode)
			{
				case 1: // Preview "Scheduled"
					status = $"Game Time: {game.GameDate:H:mm}";
					var previewPollResolution = FromMinutes(10);
					var previewRemaining = game.GameDate - _pregameOffset - DateTime.Now;
					pollTime = previewRemaining < previewPollResolution ? previewRemaining : previewPollResolution;
					break;
				case 2: // Preview "Pre-Game"
					status = $"{game.Status.DetailedState}";
					if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
						PreGameStarted?.Invoke();

					pollTime = game.GameDate > DateTime.Now ? FromMinutes(1) : FromSeconds(10);
					break;
				case 8: // Preview "Scheduled (Time TBD)"
					status = $"Game Time: TBD";
					pollTime = FromMinutes(10);
					break;
				case 3: // Live "In Progress"
					var liveStatus = myTeam.Score > opponent.Score ? "W" : myTeam.Score < opponent.Score ? "L" : "";
					status = $"{game.Status.AbstractGameState}: {liveStatus} {game.Participants.Away.Score} - {game.Participants.Home.Score}";
					if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
						PuckDropped?.Invoke();

					pollTime = FromSeconds(3);
					break;
				case 4: // Live "In Progress - Critical" | last 5 minutes
					var criticalStatus = myTeam.Score > opponent.Score ? "W" : myTeam.Score < opponent.Score ? "L" : "";
					status = $"5 To Go: {criticalStatus} {game.Participants.Away.Score} - {game.Participants.Home.Score}";
					if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
						CriticalGamePlayStarted?.Invoke();

					pollTime = FromSeconds(1.5);
					break;
				case 5: // Final "Game Over"
				case 6: // Final "Final"
				case 7: // Final "Final"
					var finalStatus = myTeam.Score > opponent.Score ? "W" : myTeam.Score < opponent.Score ? "L" : "";
					status = $"{game.Status.DetailedState}: {finalStatus} {game.Participants.Away.Score} - {game.Participants.Home.Score}";
					if (_currentGame == game.GamePk && statusCode != _lastStatusCode)
						GameEnded?.Invoke();

					pollTime = FromMinutes(1); // ToDo fix this after eval
					break;
				case 9: // Preview "Postponed"
					status = $"{game.Status.DetailedState}";
					pollTime = FromMinutes(15);
					break;
				default: throw new InvalidEnumArgumentException($"invalid {nameof(game.Status.DetailedState)}");
			}

			SetProperty(TileStatus, status);
			_extension.PrintLine($"{game.Status.AbstractGameState}");
			_lastStatusCode = statusCode;
			return pollTime;
		}

		internal IOperationResult DoCommand(string command, IEnumerable<string> parameters)
		{
			_extension.PrintLine(nameof(DoCommand));
			_extension.PrintLine($"command: {command}");
			foreach (var parameter in parameters)
				_extension.PrintLine($"parameters: {parameter}");

			switch (command)
			{
				case TileActionRoom:
					try { _extension.PrintLine($"{nameof(TileActionRoom)}"); }
					catch (Exception e) { CatchPrint(e); }
					break;

				default:
					_extension.PrintLine("NHL: Unhandled command: " + command);
					break;
			}

			return new OperationResult(OperationResultCode.Success);
		}

		internal IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
		{
			_extension.PrintLine(nameof(SetDriverPropertyValue));
			_extension.PrintLine($"propertyKey: {propertyKey}{Environment.NewLine}value: {value}");

			try
			{
				switch (propertyKey)
				{
					case SelectorButtonValueTeamKey:
						var oldValue = GetProperty<ushort>(propertyKey);
						var newV = Convert.ToUInt16(value);
						if (oldValue != newV)
						{
							SetProperty(propertyKey, newV);
							_extension.SetNumberSetting(propertyKey, newV);
							PollTimerCallback(null);
						}

						break;

					case ToggleSliderValueGoalDelayKey:
						SetProperty(propertyKey, value);
						SetProperty(ToggleSliderLabelGoalDelayKey, value.ToString());
						_extension.SetNumberSetting(propertyKey, GetProperty<ushort>(propertyKey));
						break;
				}

				return new OperationResult(OperationResultCode.Success);
			}
			catch (Exception exception)
			{
				CatchPrint(exception);
				return new OperationResult(OperationResultCode.Error);
			}
		}

		internal IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
		{
			_extension.PrintLine($"{nameof(SetDriverPropertyValue)} obj overload. id: {objectId}, prop key: {propertyKey} val: {value}");
			return new OperationResult(OperationResultCode.Success);
		}

		public void ConsoleReader(string args)
		{
			var input = args.Split(' ').Select(s => s.Trim()).ToList();
			if (input.Count < 2)
				return;

			switch (input[0])
			{
				case string log when log.Equals("log", StringComparison.OrdinalIgnoreCase):
				{
					switch (input[1])
					{
						case string on when on.Equals("on", StringComparison.OrdinalIgnoreCase):
							_extension.EnableLogging = true;
							break;
						case string off when off.Equals("off", StringComparison.OrdinalIgnoreCase):
							_extension.EnableLogging = true;
							break;
					}
					break;
				}

				case string team when team.Equals("team", StringComparison.OrdinalIgnoreCase):
				{
					var availableValue = (PropertyAvailableValue<ushort>)_teamList.FirstOrDefault(propertyAvailableValue => propertyAvailableValue.LabelLocalizationKey == input[1]);
					if (availableValue != default)
						SetDriverPropertyValue(SelectorButtonValueTeamKey, availableValue.Value);
					break;
				}
			}
		}

		public void CatchPrint(Exception exception)
		{
			_extension.PrintLine(exception.Message);
			_extension.PrintLine(exception.StackTrace);
		}

		#region IDisposable
		public void Dispose()
		{
			_extension?.PrintLine("Logic Dispose");
			_pollTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			_pollTimer?.Dispose();
			_httpTransport.Dispose();
		}
		#endregion
	}
}
