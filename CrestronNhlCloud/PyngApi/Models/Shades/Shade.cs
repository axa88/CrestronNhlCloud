using CrestronNhlCloud.PyngApi.Models.Response;

using Newtonsoft.Json;


namespace CrestronNhlCloud.PyngApi.Models.Shades
{
	public enum ShadeType
	{
		//[EnumMember(Value = "shade")]
		Shade,
	}

	public enum ShadeSubType
	{
		//[EnumMember(Value = "shade")]
		Shade,
		//[EnumMember(Value = "drape")]
		Drape,
		//[EnumMember(Value = "Shade Group")]
		ShadeGroup
	}

	public class Shade : BaseDevice
	{
		public Shade() { }

		[JsonConstructor]
		public Shade(ushort id, ushort position)
		{
			Id = id;
			Position = position;
		}

		public Shade(int id, int position)
		{
			Id = id;
			Position = position;
		}

		public Shade(Shade shade, ushort position)
		{
			Id = shade.Id;
			SubType = shade.SubType;
			ConnectionStatus = shade.ConnectionStatus;
			Name = shade.Name;
			RoomId = shade.RoomId;
			Type = shade.Type;

			Position = position;
		}

		// response hard-typed by JsonConstructor
		[JsonProperty(PropertyName = "position")]
		public int Position;

		[JsonProperty(PropertyName = "subType")]
		public ShadeSubType SubType;

		public bool ShouldSerializeSubType() { return false; }

		// new to make errant requests while response are hard-typed by JsonConstructor
		[JsonProperty(PropertyName = "id")]
		public new int Id;
	}
}
