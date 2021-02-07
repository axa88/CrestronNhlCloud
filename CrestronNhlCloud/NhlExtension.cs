using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using Crestron.RAD.Common;
using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.ExtensionMethods;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using Crestron.SimplSharp;

using CrestronNhlCloud.Transport;

using NhlApi.Models.Teams;

using static CrestronNhlCloud.NhlExtension.UiObjects;

using Timeout = System.Threading.Timeout;


namespace CrestronNhlCloud
{
	public class NhlExtension : AExtensionDevice, ICloudConnected
	{
		public static class UiObjects
		{
			public const string DefaultTeamKey = nameof(DefaultTeamKey);

			public const string TileStatus = nameof(TileStatus);
			public const string TileIcon = nameof(TileIcon);
			public const string TileSecondaryIcon = nameof(TileSecondaryIcon);
			public const string TileActionRoom = nameof(TileActionRoom);

			public const string SelectorButtonValueTeam = nameof(SelectorButtonValueTeam);
			public const string SelectorButtonLabelAction = nameof(SelectorButtonLabelAction);
		}

		private readonly Dictionary<string, IPropertyValue> _properties = new Dictionary<string, IPropertyValue>();
		private NhlTransport _nhlTransport;
		private ushort _currentTeamId;
		private readonly List<IPropertyAvailableValue> _teamList;
		private List<Team> _teams = new List<Team>();
		private Team _currentTeam;
		private Timer _pollTimer;
		private int _currentGame;
		private int _score;
		CancellationTokenSource	_cancellationTokenSource = new CancellationTokenSource();

		public NhlExtension()
		{
			CrestronConsole.PrintLine("RAD EX: cstr start");

			// Tile
			_properties[TileStatus] = CreateProperty<string>(new PropertyDefinition(TileStatus, TileStatus, DevicePropertyType.String));
			_properties[TileIcon] = CreateProperty<string>(new PropertyDefinition(TileIcon, TileIcon, DevicePropertyType.String));
			_properties[TileSecondaryIcon] = CreateProperty<string>(new PropertyDefinition(TileSecondaryIcon, TileSecondaryIcon, DevicePropertyType.String));

			// Team Selector
			_teamList = new List<IPropertyAvailableValue>();
			_properties[SelectorButtonValueTeam] = CreateProperty<ushort>(new PropertyDefinition(SelectorButtonValueTeam, SelectorButtonValueTeam, DevicePropertyType.UInt16, _teamList));

			// initial values
			((PropertyValue<string>)_properties[TileStatus]).Value = "-";
			((PropertyValue<string>)_properties[TileIcon]).Value = "icBroadcastRegular";
			((PropertyValue<string>)_properties[TileSecondaryIcon]).Value = "icSettings";
			//((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value = 1;

			Commit();
			CrestronConsole.PrintLine("RAD EX: cstr end");
		}

		[ProgrammableEvent ("^GoalScored")]
		public event Action GoalScored;

		public void Initialize()
		{
			CrestronConsole.PrintLine("RAD EXT: ini");
			_nhlTransport = new NhlTransport();
			_pollTimer = new Timer(PollTimerCallback, null, TimeSpan.FromMinutes(0), Timeout.InfiniteTimeSpan);

			Commit();
			CrestronConsole.PrintLine("RAD EX: ini'd");

		}

		protected override IOperationResult DoCommand(string command, string[] parameters)
		{
			CrestronConsole.PrintLine(nameof(DoCommand));
			CrestronConsole.PrintLine($"command: {command}");
			foreach (var parameter in parameters)
				CrestronConsole.PrintLine($"parameters: {parameter}");

			switch (command)
			{
				case TileActionRoom:
					try
					{
						CrestronConsole.PrintLine("try");
						var gameTime = DateTime.Parse("2010-08-20T15:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind);
						CrestronConsole.PrintLine($"gametime: {gameTime}");
						//TestMeth();
					}
					catch (Exception exception)
					{
						CrestronConsole.PrintLine("catch");
						CrestronConsole.PrintLine(exception.Message);
						CrestronConsole.PrintLine(exception.StackTrace);
					}

					break;

				default:
					ErrorLog.Warn("NHL: Unhandled command: " + command);
					break;
			}

			return new OperationResult(OperationResultCode.Success);
		}

		protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
		{
			CrestronConsole.PrintLine(nameof(SetDriverPropertyValue));
			CrestronConsole.PrintLine($"propertyKey: {propertyKey}");
			CrestronConsole.PrintLine($"value: {value}");

			try
			{
				switch (propertyKey)
				{
					case SelectorButtonValueTeam:
						// if team changes
						var old = ((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value;

						// set button value to selected value
						((PropertyValue<T>)_properties[SelectorButtonValueTeam]).Value = value;
						Commit();

						// store current team
						SaveSetting(nameof(DefaultTeamKey), ((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value);

						// if team changed update
						if (Convert.ToUInt16(value) != old)
						{
							CrestronConsole.PrintLine("team changed");
							PollTimerCallback(null);
						}

						break;
					default: break;
				}

				return new OperationResult(OperationResultCode.Success);
			}
			catch (Exception exception)
			{
				CrestronConsole.PrintLine($"{nameof(SetDriverPropertyValue)} catch");
				CrestronConsole.PrintLine(exception.Message);
				return new OperationResult(OperationResultCode.Error);
			}
		}

		protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
		{
			CrestronConsole.PrintLine($"{nameof(SetDriverPropertyValue)} obj overload. id: {objectId}, propkey: {propertyKey} val: {value}");
			return new OperationResult(OperationResultCode.Success);
		}

		private void PollTimerCallback(object state)
		{
			try
			{
				{ // uninitialized
					if (!_teamList.Any())
					{
						CrestronConsole.PrintLine("initialize");
						_teams = _nhlTransport.GetTeams();
						if (_teams == null)
						{
							CrestronConsole.PrintLine("cant connect to nhl");
							_pollTimer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
						}
						else
						{
							CrestronConsole.PrintLine("initialize");
							foreach (var team in _teams)
								_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));

							if (_teamList.Any())
							{
								Commit();
								Connected = true;
								CrestronConsole.PrintLine("connected");
							}
						}
					}
				}

				{ // restore default
					if (((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value == default && _teamList.Any())
					{
						CrestronConsole.PrintLine("restore");
						var defaultTeamId = GetSetting(nameof(DefaultTeamKey));
						CrestronConsole.PrintLine($"defaultTeamId: {defaultTeamId}");

						var teamId = Convert.ToUInt16(defaultTeamId);
						if (defaultTeamId != null && teamId != default)
						{
							CrestronConsole.PrintLine("check");
							((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value = teamId;
							Commit();
							CrestronConsole.PrintLine("committed");
						}
						else
						{
							CrestronConsole.PrintLine("no def");
						}

						CrestronConsole.PrintLine("done");
					}
				}

				#region function

				{ // poll
					var teamId = ((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value;
					if (teamId != default)
					{
						CrestronConsole.PrintLine("Poll");

						var schedule = _nhlTransport.GetTeamSchedule(teamId);
						if (schedule == null)
						{
							CrestronConsole.PrintLine("no sched");
							Connected = false;
							_pollTimer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
						}
						else
						{
							if (schedule.TotalGames < 1)
							{
								CrestronConsole.PrintLine("no game");
								((PropertyValue<string>)_properties[TileStatus]).Value = "No Game Today";
								Commit();
								// schedule next check at midnite
							}
							else
							{
								CrestronConsole.PrintLine("game on");

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

								((PropertyValue<string>)_properties[TileStatus]).Value = $"Game Time:{System.Environment.NewLine}{game.GameDate:hh:mm t}";
								Commit();

								// BUG changing teams in same game causes false goal. + others

								// first make sure in sync, system may have just booted or api offline
								if (_currentGame != game.GamePk)
								{
									CrestronConsole.PrintLine("trackers reset");
									_currentGame = game.GamePk;
									_score = team.Score;
								}

								if (_score != team.Score)
								{
									CrestronConsole.PrintLine(">>>>>>>>>>>>>> GOAL !!!!!!!!!!!!!!!");
									((PropertyValue<string>)_properties[TileStatus]).Value = $"GOAL!{Environment.NewLine}{game.Teams.Away.Score}-{game.Teams.Home.Score}";
									GoalScored?.Invoke();
								}

								// adjust poll timing
								if (ushort.TryParse(game.Status.StatusCode, out var statusCode) && statusCode > 6) // game over
								{
									// schedule next check at midnite
								}
								else if (game.GameDate - DateTime.Now > TimeSpan.FromMinutes(10))
								{
									CrestronConsole.PrintLine("time > 10");
									_pollTimer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
								}
								else if (game.GameDate - DateTime.Now > TimeSpan.FromMinutes(1))
								{
									CrestronConsole.PrintLine("time > 1");
									_pollTimer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
								}
								else
								{
									CrestronConsole.PrintLine("else poll");
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
				CrestronConsole.PrintLine(nameof(PollTimerCallback));
				CrestronConsole.PrintLine(exception.Message);
			}
		}
	}
}
