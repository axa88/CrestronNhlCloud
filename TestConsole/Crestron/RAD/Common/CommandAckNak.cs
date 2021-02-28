namespace Crestron.RAD.Common
{
    public class CommandAckNak
    {
        public string Ack { get; set; }
        public string Nak { get; set; }

        public CommandAckNak()
        {
            Ack = string.Empty;
            Nak = string.Empty;
        }
    }
}
