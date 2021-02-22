using System.Collections.Generic;
using Crestron.RAD.Common.ExtensionMethods;
using System;


namespace Crestron.RAD.Common
{
    public class CustomCommand
    {
        private string _command;

        public string Name { get; set; }
        public string Command
        {
            get { return _command; }
            set
            {
                _command = value.GetSafeCommandString(out CommandSetterError);
            }
        }

        /// <summary>
        /// An error message that is set when the setter for <see cref="Command"/> had an exception.
        /// This will be logged by the command sending logic if it is not null.
        /// </summary>
        internal Exception CommandSetterError;

        public IList<Parameters> Parameters { get; set; }
    }
}
