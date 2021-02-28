using System;
using System.Collections.Generic;


namespace Crestron.RAD.Common
{
	public class UserAttribute
	{
		public string TypeName { get; set; }
		public string ParameterId { get; set; }
		public string Label { get; set; }
		public string Description { get; set; }
		public bool Persistent { get; set; }
		public string RequiredForConnection { get; set; }
		public UserAttributeData Data { get; set; }
	}

	public class UserAttributeData
	{
		public string DataType { get; set; }
		public string Mask { get; set; }
		public string DefaultValue { get; set; }

		/// <summary>
		/// Default value label to display Url attribute type display text.
		/// </summary>
		public string DefaultValueLabel { get; set; }
	}

	public class UserAttributeListEventArgs : EventArgs
	{
		public UserAttributeListEventArgs(IEnumerable<UserAttribute> userAttributes)
		{
			UserAttributes = userAttributes;
		}

		public IEnumerable<UserAttribute> UserAttributes { get; private set; }
	}
}