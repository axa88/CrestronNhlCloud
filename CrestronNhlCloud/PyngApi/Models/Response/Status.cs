namespace CrestronNhlCloud.PyngApi.Models.Response
{
	public enum StatusBinary
	{
		//[EnumMember(Value = "failure")]
		Failure = StatusSync.Failure,

		//[EnumMember(Value = "success")]
		Success = StatusSync.Success
	}

	public enum StatusTrinary
	{
		//[EnumMember(Value = "failure")]
		Failure = StatusSync.Failure,

		//[EnumMember(Value = "success")]
		Success = StatusSync.Success,

		//[EnumMember(Value = "partial")]
		Partial
	}

	public enum StatusSync
	{
		Failure,
		Success
	}
}
