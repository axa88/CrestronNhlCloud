using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.DoorLocks
{
	public enum DoorLockType
	{
		//[EnumMember(Value = "lock")]
		Lock,
	}

	public enum DoorLockStatus
	{
		Locked = SyncMode.Lock,
		Unlocked = SyncMode.Unlock
	}

	public enum DoorLockMode
	{
		Lock = SyncMode.Lock,
		Unlock = SyncMode.Unlock
	}

	public class DoorLock : BaseDevice
	{
		// bug 154665, 155710
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		//[JsonProperty(PropertyName = "status")]
		public DoorLockStatus? Status;
	}

	enum SyncMode
	{
		Lock = 1,
		Unlock = 2
	}
}