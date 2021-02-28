namespace Crestron.RAD.Common
{
	public class NoAuthentication : AuthenticationNode
	{
		public override string Type => AuthenticationTypes.NONE;

		public override bool Required
		{
			get => false;
			set { }
		}
	}
}