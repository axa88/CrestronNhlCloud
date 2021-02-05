using Newtonsoft.Json;

using static System.String;


namespace CrestronNhlCloud.PyngApi.Models.Credentials
{
	public class Credentials
	{
		private ushort _port = 80;

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; } = Empty;

		[JsonProperty(PropertyName = "host")]
		public string Host { get; set; } = Empty;

		[JsonProperty(PropertyName = "port")]
		public ushort Port
		{
			get => _port;
			set => _port = (ushort)(value == default ? 80 : value);
		}

		[JsonProperty(PropertyName = "hideAddress")]
		public bool HideAddress { get; set; }

		[JsonProperty(PropertyName = "token")]
		public string Token { get; set; } = Empty;

		[JsonProperty(PropertyName = "timeout")]
		public int Timeout { get; set; } = 8_000;
	}
}
