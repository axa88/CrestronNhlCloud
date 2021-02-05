using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Errors
{
	public enum ErrorSource : short
	{
		Authentication = 5002,
		Devices = 8001,
		Login = 7001,
		Lights = 7003,
		Logout = 7005,
		DoorLocks = 8007,
		FanMode = 7008,
		InvalidData = 8000,
		MediaRooms = 8010,
		Rooms = 6001,
		Scenes = 7006,
		Scheduler = 8008,
		SecurityDevices = 8005,
		Sensors = 8006,
		SessionExpired = 5001,
		SetPoint = 8009,
		Shades = 7004,
		SystemMode = 7009,
		Thermostats = 7007,
		Unhandled = 7000,
	}


	public class Error
	{
		[JsonIgnore]
		public int HttpStatusCode { get; set; }

		[JsonIgnore]
		public bool Success { get; set; }

		[JsonIgnore]
		[JsonProperty(PropertyName = "errorSource")]
		public ErrorSource ErrorSource { get; set; }

		[JsonIgnore]
		[JsonProperty(PropertyName = "errorMessage")]
		public string ErrorMessage { get; set; }

		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
	}
}
