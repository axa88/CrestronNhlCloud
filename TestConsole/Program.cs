using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Api.Models.Teams;

using CrestronNhlCloud.NhlApiShared.Application;

using NhlApiShared;

using TestConsole.Portability;
using TestConsole.Transport;


namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var apiTransport = new HttpTransport();
			var schedule = apiTransport.GetTeamSchedule(3);
			var teams = apiTransport.GetTeams();

			Console.WriteLine(schedule.Copyright);
			Console.WriteLine(teams.Count);

			Console.ReadLine();
		}
	}

	public class NhlExtension : IApplication, IPlatform, ISettings
	{
		public HttpTransport HttpTransport;
		private Logic _logic;

		private readonly Dictionary<string, IPropertyValue> _properties = new Dictionary<string, IPropertyValue>();
		private readonly List<IPropertyAvailableValue> _teamList;

		#region Implementation of IPlatform
		public T GetProperty<T>(string key) => ((PropertyValue<T>)_properties[key]).Value;
		public void SetProperty<T>(string key, T value) => ((PropertyValue<T>)_properties[key]).Value = value;
		public void UpdateUi() { }
		public void Connect(bool state) {  }
		#endregion

		#region Implementation of ISettings
		public void PrintLine(string message) => Console.WriteLine(message);
		public bool GetBoolSetting(string key) { return default; }
		public void SetBoolSetting(string key, bool value) { }
		public ushort GetNumberSetting(string key) { return default; }
		public void SetNumberSetting(string key, ushort value) { }
		public string GetStringSetting(string key) { return default; }
		public void SetStringSetting(string key, string value) { }
		#endregion

		#region Implementation of IApplication
		public IEnumerable<object> TeamList => new List<object>(_teamList);

		public void AddTeams(IEnumerable<Team> teams)
		{
			foreach (var team in teams)
				_teamList.Add(new PropertyAvailableValue<ushort>(team.Id, DevicePropertyType.UInt16, $"{team.Abbreviation}", null));
		}
		#endregion
	}
}