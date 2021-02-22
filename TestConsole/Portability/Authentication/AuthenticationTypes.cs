using System.Collections.Generic;


namespace Crestron.RAD.Common
{
    public static class AuthenticationTypes
    {
        public const string NONE = "None";
        public const string USERNAME_PASSWORD = "UsernamePassword";

        public static List<string> ValidAuthenticationTypes;

        static AuthenticationTypes()
        {
            ValidAuthenticationTypes = new List<string>();
            ValidAuthenticationTypes.Add(NONE);
            ValidAuthenticationTypes.Add(USERNAME_PASSWORD);
        }
    }
}