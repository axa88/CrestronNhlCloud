using System.Collections.Generic;

using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Media
{
	public enum MediaType
	{
		//[EnumMember(Value = "Media")]
		Media
	}

	public enum MuteCommand
	{
		//[EnumMember(Value = "mute")]
		Mute = SyncMode.Mute,
		//[EnumMember(Value = "unmute")]
		UnMute = SyncMode.Unmute,
	}

	public enum VolumeDiscreteCommand
	{
		//[EnumMember(Value = "volume")]
		Volume
	}

	public enum SourceCommand
	{
		//[EnumMember(Value = "selectsource")]
		SelectSource
	}

	public enum PowerCommand
	{
		//[EnumMember(Value = "power")]
		Power
	}

	public enum CurrentMuteState
	{
		//[EnumMember(Value = "muted")]
		Muted = SyncMode.Mute,
		//[EnumMember(Value = "unmuted")]
		Unmuted = SyncMode.Unmute
	}

	public enum MuteControl
	{
		//[EnumMember(Value = "none")]
		None,
		//[EnumMember(Value = "discrete")]
		Discrete,
		//[EnumMember(Value = "toggle")]
		Toggle
	}

	public enum VolumeControl
	{
		//[EnumMember(Value = "none")]
		None,
		//[EnumMember(Value = "discrete")]
		Discrete,
		//[EnumMember(Value = "relative")]
		Relative
	}

	public enum RelativeControl
	{
		Up,
		Down
	}

	public enum PowerState
	{
		//[EnumMember(Value = "off")]
		Off,
		//[EnumMember(Value = "on")]
		On
	}

	public class AvailableSource : Base
	{
		[JsonProperty(PropertyName = "sourceName")]
		public new string Name { get; set; }
	}

	public class MediaRoom : /*BaseResponse*/ BaseDevice // MediaRoom doesn't have Connection status of BaseDevice, but using it gives a common type used in SubSystemActivity
	{
		[JsonProperty(PropertyName = "availableSources")]
		public List<AvailableSource> AvailableSources { get; set; }

		[JsonProperty(PropertyName = "availableVolumeControls")]
		public List<VolumeControl> AvailableVolumeControls { get; set; }

		[JsonProperty(PropertyName = "availableMuteControls")]
		public List<MuteControl> AvailableMuteControls { get; set; }

		[JsonProperty(PropertyName = "currentMuteState")]
		public CurrentMuteState CurrentMuteState { get; set; }

		[JsonProperty(PropertyName = "currentPowerState")]
		public PowerState CurrentPowerState { get; set; }

		[JsonProperty(PropertyName = "currentSourceId")]
		public ushort CurrentSourceId { get; set; }

		[JsonProperty(PropertyName = "currentVolumeLevel")]
		public ushort CurrentVolumeLevel { get; set; }
	}

	internal enum SyncMode
	{
		Mute = 1,
		Unmute = 2
	}
}
