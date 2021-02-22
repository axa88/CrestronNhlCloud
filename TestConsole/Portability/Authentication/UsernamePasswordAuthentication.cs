using System;
using System.Text.RegularExpressions;


namespace Crestron.RAD.Common
{
	public class UsernamePasswordAuthentication : AuthenticationNode
	{
		#region Properties

		public override string Type
		{
			get { return AuthenticationTypes.USERNAME_PASSWORD; }
		}

		public override bool Required { get; set; }

		public bool UsernameRequired;

		[Obsolete("This is deprecated.", false)]
		public string UsernameMask { get; set; } = string.Empty;

		public bool PasswordRequired;

		[Obsolete("This is deprecated.", false)]
		public string PasswordMask { get; set; } = string.Empty;

		public string DefaultUsername { get; set; }

		public string DefaultPassword { get; set; }

		#endregion

		public UsernamePasswordAuthentication()
		{
			DefaultPassword = string.Empty;
			DefaultUsername = string.Empty;
		}

		#region Methods

		[Obsolete("This method is deprecated because UsernameMask and PasswordMask are deprecated.", false)]
		private bool IsValidRegex(string pattern)
		{
			bool isValid = false;
			try
			{
				var temp = new Regex(pattern);
				isValid = true;
			}
			catch (ArgumentException)
			{
				//should alert user of invalid regex
				isValid = false;
			}
			return isValid;
		}

		#endregion
	}
}