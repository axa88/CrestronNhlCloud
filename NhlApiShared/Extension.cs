using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using NhlApiShared.Api.Models.Schedule;

using static System.TimeSpan;

using static NhlApiShared.Common.UiObjects;

using Team = NhlApiShared.Api.Models.Teams.Team;

#if WINDOWS
using TestConsole.Portability;
using TestConsole.Transport;
#endif

#if CRESTRON
using CrestronNhlCloud.Transport;
#endif


public class Extension : AExtensionDevice, ICloudConnected
{
	private readonly Dictionary<string, IPropertyValue> _properties = new Dictionary<string, IPropertyValue>();
	private readonly List<IPropertyAvailableValue> _teamList = new List<IPropertyAvailableValue>();

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

	[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
	public Extension()
	{
		try
		{
			EnableLogging = true;
			Log("Logic ctor start");
			Log("Logic ctor end");
		}
		catch (Exception exception)
		{
			Log(exception.Message);
		}
	}

	[ProgrammableEvent("^PreGameStarted")]
	public event Action PreGameStarted;

	[ProgrammableEvent("^PuckDropped")]
	public event Action PuckDropped;

	[ProgrammableEvent("^CriticalGamePlayStarted")]
	public event Action CriticalGamePlayStarted;

	[ProgrammableEvent("^GameEnded")]
	public event Action GameEnded;

	[ProgrammableEvent("^TeamGoalScored")]
	public event Action TeamGoalScored;

	[ProgrammableEvent("^OpponentGoalScored")]
	public event Action OpponentGoalScored;

	public override void Dispose()
	{
		Log("Logic Dispose");
		_pollTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
		_goalDelayTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
		_opponentDelayTimer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
		_pollTimer?.Dispose();
		_goalDelayTimer?.Dispose();
		_opponentDelayTimer?.Dispose();
		_httpTransport?.Dispose();
		base.Dispose();
	}

	public void Initialize()
	{
		try
		{
			Log("ini ini start");

			// Create
			// Tile
			_properties[TileStatus] = CreateProperty<string>(TileStatus, DevicePropertyType.String);
			_properties[TileIcon] = CreateProperty<string>(TileIcon, DevicePropertyType.String);
			//Properties[TileSecondaryIcon] = _extension.CreateProp<string>(TileSecondaryIcon, DevicePropertyType.String);

			// Team Selector
			_properties[SelectorButtonValueTeamKey] = CreateProperty<ushort>(SelectorButtonValueTeamKey, DevicePropertyType.UInt16, _teamList);

			// Goal Delay
			// Label
			_properties[ToggleSliderLabelGoalDelayKey] = CreateProperty<string>(ToggleSliderLabelGoalDelayKey, DevicePropertyType.String);
			// Slider
			_properties[ToggleSliderValueGoalDelayKey] = CreateProperty<ushort>(ToggleSliderValueGoalDelayKey, DevicePropertyType.UInt16, 0, 60, 1);

			// initial values
			SetProperty(TileStatus, "-");
			SetProperty(TileIcon, "icBroadcastRegular");
			//ChangeProperty(TileSecondaryIcon, "icSettings");
			SetProperty(ToggleSliderValueGoalDelayKey, GetNumberSetting(ToggleSliderValueGoalDelayKey));
			SetProperty(ToggleSliderLabelGoalDelayKey, GetProperty<ushort>(ToggleSliderValueGoalDelayKey).ToString());

			// assign event handlers
			PreGameStarted += PreGameStarted;
			PuckDropped += PuckDropped;
			CriticalGamePlayStarted += CriticalGamePlayStarted;
			GameEnded += GameEnded;
			TeamGoalScored += TeamGoalScored;
			OpponentGoalScored += OpponentGoalScored;

			_httpTransport = new HttpTransport();

			_goalDelayTimer = new Timer(state => { TeamGoalScored?.Invoke(); }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			_opponentDelayTimer = new Timer(state => { OpponentGoalScored?.Invoke(); }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			_pollTimer = new Timer(PollTimerCallback, null, FromMinutes(0), Timeout.InfiniteTimeSpan);

			Log("ini ini end");
		}
		catch (Exception exception)
		{
			Log(exception.Message);
			Log(exception.StackTrace);
		}
	}

	protected override IOperationResult DoCommand(string command, string[] parameters)
	{
		Log(nameof(DoCommand));
		Log($"command: {command}");

		foreach (var parameter in parameters)
			Log($"parameters: {parameter}");

		switch (command)
		{
			case TileActionRoom:
				try { Log($"{nameof(TileActionRoom)}"); }
				catch (Exception e) { CatchPrint(e); }
				break;

			default:
				Log("NHL: Unhandled command: " + command);
				break;
		}

		return new OperationResult(OperationResultCode.Success);
	}

	protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
	{
		Log(nameof(SetDriverPropertyValue));
		Log($"propertyKey: {propertyKey}{Environment.NewLine}value: {value}");

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
						SetNumberSetting(propertyKey, newV);
						PollTimerCallback(null);
					}

					break;

				case ToggleSliderValueGoalDelayKey:
					SetProperty(propertyKey, value);
					SetProperty(ToggleSliderLabelGoalDelayKey, value.ToString());
					SetNumberSetting(propertyKey, GetProperty<ushort>(propertyKey));
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

	protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
	{
		Log($"{nameof(SetDriverPropertyValue)} obj overload. id: {objectId}, prop key: {propertyKey} val: {value}");
		return new OperationResult(OperationResultCode.Success);
	}

	private T GetProperty<T>(string key) => ((PropertyValue<T>)_properties[key]).Value;

	private void SetProperty<T>(string key, T value)
	{
		((PropertyValue<T>)_properties[key]).Value = value;
		Commit();
	}

	private PropertyValue<T> CreateProperty<T>(string key, DevicePropertyType type, IEnumerable<IPropertyAvailableValue> availableValues = null)
		=> CreateProperty<T>(availableValues == null ? new PropertyDefinition(key, key, type) : new PropertyDefinition(key, key, type, availableValues));

	private PropertyValue<T> CreateProperty<T>(string key, DevicePropertyType type, ushort min, ushort max, ushort step)
		=> CreateProperty<T>(new PropertyDefinition(key, key, type, min, max, step));

	private void PollTimerCallback(object state)
	{
		Log($"{nameof(PollTimerCallback)} start :: {DateTime.Now.ToShortDateString()} :: {DateTime.Now.ToShortTimeString()}");
		try
		{
			lock (_pollLock)
			{
				Connected = PollInitializationLogic(); // todo replace with connectivity service

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
				Log("cant get teams");
		}

		if (_teamList.Any() && GetProperty<ushort>(SelectorButtonValueTeamKey) == default)
		{
			Log("restore");
			var recalledTeamId = GetNumberSetting(nameof(SelectorButtonValueTeamKey));
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
							Log("game update");
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
					Log("no game data");
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
			Log("no schedule");
			pollTime = FromMinutes(10);
		}

		return pollTime;
	}

	private void AddTeams(IEnumerable<Team> teams)
	{
		foreach (var team in teams)
			_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));

		if (_teamList.Any())
			Commit();
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
			default:
				status = "unknown";
				pollTime = FromMinutes(15);
				break;
		}

		SetProperty(TileStatus, status);
		Log($"{game.Status.AbstractGameState}");
		_lastStatusCode = statusCode;
		return pollTime;
	}

	private bool GetBoolSetting(string key) => Convert.ToBoolean(GetSetting(key));
	private void SetBoolSetting(string key, bool value) => SaveSetting(key, value);
	private ushort GetNumberSetting(string key) => Convert.ToUInt16(GetSetting(key));
	private void SetNumberSetting(string key, ushort value) => SaveSetting(key, value);
	private string GetStringSetting(string key) => GetSetting(key).ToString();
	private void SetStringSetting(string key, string value) => SaveSetting(key, value);

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
						EnableLogging = true;
						break;
					case string off when off.Equals("off", StringComparison.OrdinalIgnoreCase):
						EnableLogging = true;
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

	private void CatchPrint(Exception exception)
	{
		Log(exception.Message);
		Log(exception.StackTrace);
	}
}