using System;
using System.Collections.Generic;

using Newtonsoft.Json.Serialization;

using System.Reflection;


namespace Crestron.RAD.Common.ContractResolvers
{
	/// <summary>
	/// Contract resolver that will filter the output of <see cref="GetSerializableMembers"/> based on a list
	/// of Types to ignore.
	/// </summary>
	public class IgnoreTypeContractResolver : DefaultContractResolver
	{
		private Dictionary<string, Type> _ignoreList = new Dictionary<string, Type>();

		/// <summary>
		/// Adds a Type to ignore when getting serializable members.
		/// Each type is associated with one unique name.
		/// </summary>
		/// <param name="type">Declaring type to ignore that has the specified name.</param>
		/// <param name="name">Name to ignore.</param>
		public void IgnoreType(string name, Type type)
		{
			if (!string.IsNullOrEmpty(name) && type != null)
				_ignoreList[name] = type;
		}

		/// <summary>
		/// Removes all instances of MemberInfo where the declaring type was added using <see cref="IgnoreType"/>.
		/// </summary>
		protected override List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			var serializableMembers = base.GetSerializableMembers(objectType);

			foreach (var ignoredPair in _ignoreList)
			{
				var indexToRemove = serializableMembers.FindIndex(x =>
					x.Name.Equals(ignoredPair.Key) &&
					x.DeclaringType == ignoredPair.Value);

				if (indexToRemove > -1)
					serializableMembers.RemoveAt(indexToRemove);
			}

			return serializableMembers;
		}
	}
}
