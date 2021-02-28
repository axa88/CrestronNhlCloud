using System.Collections.Generic;
using Crestron.RAD.Common.Enums;


namespace Crestron.RAD.Common
{
    public class Parameters
    {
        public string Id { get; set; }
        public short Max { get; set; }
        public short Min { get; set; }
        public Types Type;
        public short StaticDataWidth { get; set; }
        public string PadCharacter;
        public enum PadDirections { Left, Right }
        public PadDirections PadDirection;
        public string value { get; set; }

        public enum Types
        {
            String = 0,
            AsciiToHex = 1,
            DecimalToHex = 2,
            HexString = 3
            // Future conversion types can be added here
        }
    }

    public class Commands
    {
        public StandardCommandsEnum StandardCommand { get; set; }
        public string Command { get; set; }
        public IList<Parameters> Parameters { get; set; }

        /// <summary>
        /// Avr Zone Field to allow for Sending of zone related commands
        /// </summary>
        public bool AllowIsSendableOverride { get; set; }

        /// <summary>
        /// Avr Zone Field to allow for Queuing of zone related commands
        /// </summary>
        public bool AllowIsQueueableOverride { get; set; }
    }
}
