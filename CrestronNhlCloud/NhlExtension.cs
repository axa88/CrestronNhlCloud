using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using Crestron.SimplSharp;

using CrestronNhlCloud.PyngApi.Models.Credentials;
using CrestronNhlCloud.Transport;

using static CrestronNhlCloud.NhlExtension.UiObjects;

using Timeout = System.Threading.Timeout;

using static CrestronNhlCloud.PyngApi.ApiClients.RestClient;

namespace CrestronNhlCloud
{
	public class NhlExtension : AExtensionDevice, ICloudConnected
	{
		private readonly Dictionary<string, IPropertyValue> _properties = new Dictionary<string, IPropertyValue>();
		private HttpApiTransport _httpApiTransport;
		private IPropertyAvailableValue _currentTeam;
		private ushort _currentTeamId;
		private readonly List<IPropertyAvailableValue> _teamList;

		private Timer _pollTimer;
		private int _currentGame;
		private int _score;

		CancellationTokenSource	_cancellationTokenSource = new CancellationTokenSource();


		public static class UiObjects
		{
			public const string TileStatus = nameof(TileStatus);
			public const string TileIcon = nameof(TileIcon);
			public const string TileSecondaryIcon = nameof(TileSecondaryIcon);
			public const string TileActionRoom = nameof(TileActionRoom);

			public const string SelectorButtonValueTeam = nameof(SelectorButtonValueTeam);
			public const string SelectorButtonLabelAction = nameof(SelectorButtonLabelAction);
		}


		public NhlExtension()
		{
			// Tile
			_properties[TileStatus] = CreateProperty<string>(new PropertyDefinition(UiObjects.TileStatus, UiObjects.TileStatus, DevicePropertyType.String));
			_properties[TileIcon] = CreateProperty<string>(new PropertyDefinition(TileIcon, TileIcon, DevicePropertyType.String));
			_properties[TileSecondaryIcon] = CreateProperty<string>(new PropertyDefinition(TileSecondaryIcon, TileSecondaryIcon, DevicePropertyType.String));

			// Team Selector
			_teamList = new List<IPropertyAvailableValue>();
			_properties[SelectorButtonValueTeam] = CreateProperty<ushort>(new PropertyDefinition(SelectorButtonValueTeam, SelectorButtonValueTeam, DevicePropertyType.UInt16, _teamList));

			Refresh();
			Commit();

			CrestronConsole.PrintLine("RAD EX: cstr");
		}

		public void Initialize()
		{
			_httpApiTransport = new HttpApiTransport();
			_pollTimer = new Timer(PollTimerCallback, null, TimeSpan.FromMinutes(0), Timeout.InfiniteTimeSpan);

			Commit();
			CrestronConsole.PrintLine("RAD EX: inid");
		}

		private void PollTimerCallback(object state)
		{
			// uninitialized
			if (!_teamList.Any())
			{
				var teams = _httpApiTransport.GetTeams();
				if (teams != null)
				{
					foreach (var team in teams)
						_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));

					if (_teamList.Any())
						Connected = true;
				}
			}

			var teamId = ((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value;
			if (teamId != default)
			{
				var schedule = _httpApiTransport.GetTeamSchedule(teamId);
				if (schedule != null && schedule.TotalGames > 0)
				{
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

					var game = schedule.Dates.First().Games.First();

					var team = game.Teams.Away.Team.Id == teamId ? game.Teams.Away : game.Teams.Home;

					// first make sure in sync, system may have just booted or api offline
					if (_currentGame != game.GamePk)
						_score = team.Score;

					if (_score != team.Score)
					{
						// do Goal Action
					}

					// adjust poll timing
					if (game.GameDate - DateTime.Now > TimeSpan.FromMinutes(10))
						_pollTimer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
					else
					{
						if (game.Teams.Away.Team.Id == teamId)
						{

						}
						else
						{

						}

						_pollTimer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
					}

					var gameTime = DateTime.Parse("2010-08-20T15:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind);
				}
				else
					((PropertyValue<string>)_properties[TileStatus]).Value = "No Game Today";
			}

		}

		protected override IOperationResult DoCommand(string command, string[] parameters)
		{
			switch (command)
			{
				case TileActionRoom:
					CrestronConsole.PrintLine(TileActionRoom);

					try
					{
						CrestronConsole.PrintLine("trying");

						//TestMeth();
					}
					catch (Exception exception)
					{
						((PropertyValue<string>)_properties[TileStatus]).Value = "catch";
						CrestronConsole.PrintLine(exception.Message);
						CrestronConsole.PrintLine(exception.StackTrace);

					}

					break;

				default:
					ErrorLog.Warn("NHL: Unhandled command: " + command);
					break;
			}

			Commit();
			CrestronConsole.PrintLine("committed");
			return new OperationResult(OperationResultCode.Success);
		}

		protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
		{
			CrestronConsole.PrintLine($"propertyKey {propertyKey}");
			CrestronConsole.PrintLine($"value {value}");

			try
			{

				switch (propertyKey)
				{
						case SelectorButtonValueTeam:
						/*var v = ((PropertyValue<T>)_properties[propertyKey]).Value;
						uint.TryParse(v, out var teamId);
						_currentTeamId = teamId;*/

						((PropertyValue<T>)_properties[SelectorButtonValueTeam]).Value = value;

						break;
					default: break;
				}

				Commit();
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

		//private void SetTeamList()
		//{
		//	var teams = _httpApiTransport.GetTeams();

		//	foreach (var team in teams)
		//		_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.String, $"{team.Abbreviation}", null));

		//	((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value = 3;
		//}

		private void Refresh()
		{
			((PropertyValue<string>)_properties[TileStatus]).Value = "-";
			((PropertyValue<string>)_properties[TileIcon]).Value = "icBroadcastRegular";
			((PropertyValue<string>)_properties[TileSecondaryIcon]).Value = "icSettings";

			//((PropertyValue<ushort>)_properties[SelectorButtonValueTeam]).Value = 1;
		}

		private void TestMeth()
		{
			CrestronConsole.PrintLine("meth");
			var credentials = new Credentials { Host = "192.168.1.63", Name = "me", Token = "9mR2kSxXjhhy", Timeout = 3000 };

			try
			{
				_cancellationTokenSource = new CancellationTokenSource(credentials.Timeout);

				var session = UpdateSession(credentials, _cancellationTokenSource.Token);
				((PropertyValue<string>)_properties[TileStatus]).Value = session.Success ? "Success" : session.ErrorMessage;

				CrestronConsole.PrintLine("meth tried ok");

			}
			catch (Exception e)
			{
				CrestronConsole.PrintLine("meth catch");

				var NewLine = Environment.NewLine;
				var s = e is OperationCanceledException _
					? $"Hostname lookup is taking too long{NewLine}If your network cannot resolve Hostnames, use IP addresses"
					: $"Oops, something went wrong.{NewLine}¯\\_(ツ)_/¯{NewLine}{e.Message}";

				((PropertyValue<string>)_properties[TileStatus]).Value = s;
			}

		}
	}
}
