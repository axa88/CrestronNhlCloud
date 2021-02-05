

namespace PyngApi.Serialization
{
	public class CustomNewtonsoftJsonSerializer : IDeserializer
	{
		public static CustomNewtonsoftJsonSerializer Instance => new CustomNewtonsoftJsonSerializer();

		public T Deserialize<T>(IRestResponse response) => RestSharpResponseHandlers.DeserializeObject<T>(response);
	}
}