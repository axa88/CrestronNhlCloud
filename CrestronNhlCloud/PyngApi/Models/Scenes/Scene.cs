using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Scenes
{
	public enum SceneType
	{
		Nil,
		Lighting,
		Climate,
		Shade,
		//[EnumMember(Value = "IO")]
		Io,
		Daylight,
		Lock,
		//[EnumMember(Value = "genericIO")]
		GenericIo,

		// bug 154700
		Unlock,
		LockRoom,

		Media,
	}

	public enum SceneStatus
	{
		Active,
		Inactive
	}

	public class Scene : BaseResponse
	{
		public Scene() { }

		public Scene(SceneV2 scene)
		{
			SceneType = scene.SceneType;
			Name = scene.Name;
			Status = scene.Status;
			Id = scene.Id;
			RoomId = scene.RoomId;
		}

		// new because base prevents serialization
		[JsonProperty(PropertyName = "name")]
		public new string Name;

		[JsonIgnore]
		[JsonProperty(PropertyName = "subType")]
		public SceneType SceneType;

		[JsonProperty(PropertyName = "status")]
		// bug 154681
		// public SceneStatus Status;
		public bool Status;
	}

	public class SceneV2 : Scene
	{
		[JsonProperty(PropertyName = "type")]
		public new SceneType SceneType;
	}
}
