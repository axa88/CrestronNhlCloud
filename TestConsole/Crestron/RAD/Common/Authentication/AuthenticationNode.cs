namespace Crestron.RAD.Common
{
    public abstract class AuthenticationNode
    {
        public abstract string Type { get; }

        public abstract bool Required { get; set; }
    }
}