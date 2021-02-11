using System;
using System.Collections.Generic;
using System.Threading;

using Crestron.RAD.Common;
using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.CloudReporting;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.ExtensionMethods;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;

using Crestron.SimplSharp;

using CrestronNhlCloud.NhlApiShared.Application;
using CrestronNhlCloud.Transport;

using Api.Models.Teams;

using static NhlApiShared.Application.UiObjects;


namespace CrestronNhlCloud
{
	public class NhlExtension : AExtensionDevice, ICloudConnected
	{
		public HttpTransport HttpTransport;
		private Logic _logic;

		private readonly Dictionary<string, IPropertyValue> _properties = new Dictionary<string, IPropertyValue>();
		private readonly List<IPropertyAvailableValue> _teamList;

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

		// Platform
		public void PrintLine(string message) => CrestronConsole.PrintLine(message);
		public bool GetBoolSetting(string key) => Convert.ToBoolean(GetSetting(key));
		public void SetBoolSetting(string key, bool value) => SaveSetting(key, value);
		public ushort GetNumberSetting(string key) => Convert.ToUInt16(GetSetting(key));
		public void SetNumberSetting(string key, ushort value) => SaveSetting(key, value);
		public string GetStringSetting(string key) => GetSetting(key).ToString();
		public void SetStringSetting(string key, string value) => SaveSetting(key, value);
		public void UpdateUi() => Commit();
		public void Connect(bool state) => Connected = state;

		// Application
		public IEnumerable<object> TeamList => new List<object>(_teamList);

		public void AddTeams(IEnumerable<Team> teams)
		{
			foreach (var team in teams)
				_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));
		}

		public T GetProperty<T>(string key) => ((PropertyValue<T>)_properties[key]).Value;

		public void SetProperty<T>(string key, T value) => ((PropertyValue<T>)_properties[key]).Value = value;

		public void Initialize()
		{
			CrestronConsole.PrintLine("RAD EXT: ini");
			HttpTransport = new HttpTransport();
			//_pollTimer = new Timer(PollTimerCallback, this, TimeSpan.FromMinutes(0), Timeout.InfiniteTimeSpan);

			_logic = new Logic(this);
			_logic.GoalScored += () => GoalScored?.Invoke();

			//Commit();
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
							_logic.PollTimerCallback(this);
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


		/*private void PollTimerCallback(object platformObject)
		{
			if (!(platformObject is NhlExtension parent))
				return;

			try
			{
				{ // uninitialized
					if (!parent._teamList.Any())
					{
						CrestronConsole.PrintLine("initialize");
						parent._teams = parent.HttpTransport.GetTeams();
						if (parent._teams == null)
						{
							CrestronConsole.PrintLine("cant connect to nhl");
							parent._pollTimer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
						}
						else
						{
							CrestronConsole.PrintLine("initialize");
							foreach (var team in parent._teams)
								parent._teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));

							if (parent._teamList.Any())
							{
								Commit();
								Connected = true;
								CrestronConsole.PrintLine("connected");
							}
						}
					}
				}

				{ // restore default
					if (((PropertyValue<ushort>)parent._properties[SelectorButtonValueTeam]).Value == default && parent._teamList.Any())
					{
						CrestronConsole.PrintLine("restore");
						var defaultTeamId = GetSetting(nameof(DefaultTeamKey));
						CrestronConsole.PrintLine($"defaultTeamId: {defaultTeamId}");

						var teamId = Convert.ToUInt16(defaultTeamId);
						if (defaultTeamId != null && teamId != default)
						{
							CrestronConsole.PrintLine("check");
							((PropertyValue<ushort>)parent._properties[SelectorButtonValueTeam]).Value = teamId;
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
					var teamId = ((PropertyValue<ushort>)parent._properties[SelectorButtonValueTeam]).Value;
					if (teamId != default)
					{
						CrestronConsole.PrintLine("Poll");

						var schedule = parent.HttpTransport.GetTeamSchedule(teamId);
						if (schedule == null)
						{
							CrestronConsole.PrintLine("no sched");
							Connected = false;
							parent._pollTimer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
						}
						else
						{
							if (schedule.TotalGames < 1)
							{
								CrestronConsole.PrintLine("no game");
								((PropertyValue<string>)parent._properties[TileStatus]).Value = "No Game Today";
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
							}#1#

								/*"status": {
								"abstractGameState": "Final",
								"codedGameState": "7",
								"detailedState": "Final",
								"statusCode": "7",
								"startTimeTBD": true
							}#1#
								/*"status" : {
								"abstractGameState" : "Preview",
								"codedGameState" : "9",
								"detailedState" : "Postponed",
								"statusCode" : "9",
								"startTimeTBD" : false
							}#1#
								#endregion

								var game = schedule.Dates.First().Games.First();
								var team = game.Teams.Away.Team.Id == teamId ? game.Teams.Away : game.Teams.Home;

								((PropertyValue<string>)parent._properties[TileStatus]).Value = $"Game Time:{Environment.NewLine}{game.GameDate:hh:mm t}";
								Commit();

								// BUG changing teams in same game causes false goal. + others

								// first make sure in sync, system may have just booted or api offline
								if (parent._currentGame != game.GamePk)
								{
									CrestronConsole.PrintLine("trackers reset");
									parent._currentGame = game.GamePk;
									parent._score = team.Score;
								}

								if (parent._score != team.Score)
								{
									CrestronConsole.PrintLine(">>>>>>>>>>>>>> GOAL !!!!!!!!!!!!!!!");
									((PropertyValue<string>)parent._properties[TileStatus]).Value = $"GOAL!{Environment.NewLine}{game.Teams.Away.Score}-{game.Teams.Home.Score}";
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
									parent._pollTimer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
								}
								else if (game.GameDate - DateTime.Now > TimeSpan.FromMinutes(1))
								{
									CrestronConsole.PrintLine("time > 1");
									parent._pollTimer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
								}
								else
								{
									CrestronConsole.PrintLine("else poll");
									parent._pollTimer.Change(TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
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
		}*/
	}
}
