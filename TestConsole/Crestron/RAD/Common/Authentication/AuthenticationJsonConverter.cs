using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Crestron.RAD.Common
{
	public class AuthenticationJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(AuthenticationNode));
		}

		private bool TokensExist(JObject jsonObject, IEnumerable<string> tokenNames) => tokenNames.All(token => TokenExists(jsonObject, token));

		private bool TokenExists(JObject jsonObject, string tokenName) => jsonObject[tokenName] != null;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jo = JObject.Load(reader);
			object result = jo;
			if (TokenExists(jo, "Type"))
			{
				if (jo["Type"].Value<string>().Equals(AuthenticationTypes.NONE))
					result = CreateNoAuthentication(jo);

				if (jo["Type"].Value<string>().Equals(AuthenticationTypes.USERNAME_PASSWORD))
					result = CreateUsernamePasswordAuth(jo);
			}
			return result;
		}

		private object CreateNoAuthentication(JObject jo) => TokenExists(jo, "Required") ? new NoAuthentication { Required = jo["Required"].Value<bool>() } : new NoAuthentication();

		private object CreateUsernamePasswordAuth(JObject jo)
		{
			var usernamePasswordAuth = new UsernamePasswordAuthentication();
			if(TokensExist(jo, new List<string>{"UsernameRequired", "UsernameMask", "PasswordRequired", "PasswordMask"}))
			{
				usernamePasswordAuth.UsernameRequired = jo["UsernameRequired"].Value<bool>();
				usernamePasswordAuth.UsernameMask = jo["UsernameMask"].Value<string>();
				usernamePasswordAuth.PasswordRequired = jo["PasswordRequired"].Value<bool>();
				usernamePasswordAuth.PasswordMask = jo["PasswordMask"].Value<string>();
			}
			if (TokensExist(jo, new List<string>{"DefaultUsername", "DefaultPassword", "Required"}))
			{
				usernamePasswordAuth.DefaultUsername = jo["DefaultUsername"].Value<string>();
				usernamePasswordAuth.DefaultPassword = jo["DefaultPassword"].Value<string>();
				usernamePasswordAuth.Required = jo["Required"].Value<bool>();
			}
			return usernamePasswordAuth;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
