using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Thermostats
{
	public enum SchedulerState
	{
		//[EnumMember(Value = "hold")]
		Hold,

		//[EnumMember(Value = "off")]
		Off,

		//[EnumMember(Value = "run")]
		Run
	}

	public class Schedule
	{
		public Schedule(int id, SchedulerState schedulerState)
		{
			Id = id;
			SchedulerState = schedulerState;
		}

		[JsonProperty(PropertyName = "id")]
		public int Id;

		[JsonProperty(PropertyName = "schedulerState")]
		public SchedulerState SchedulerState;
	}
}
