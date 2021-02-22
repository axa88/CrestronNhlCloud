using System;
using System.Threading;

using CTimer = System.Threading.Timer;


namespace Crestron.RAD.Common.BasicDriver
{
    /// <summary>
    /// Used for TX and general driver maintenance
    /// </summary>
    public static class DriverClock
    {
        /// <summary>
        /// Clock rate used when drivers are depending on this class.
        /// </summary>
        internal static uint Clock25ms = 25;

        /// <summary>
        /// Clock rate to use when there are no drivers loaded.
        /// </summary>
        internal static uint IdleClock5000ms = 5000;

        /// <summary>
        /// Single CTimer instance to start a new thread.
        /// </summary>
        internal static CTimer Driver25msClock = new CTimer(Driver25msClockCallback, null, Clock25ms, Timeout.Infinite);

        /// <summary>
        /// Used by listeners to be notified of a clock event so that they can process their clock-based logic.
        /// </summary>
        internal static Action Driver25msClockEventHandler;

        /// <summary>
        /// Specifies if the program is stopping.
        /// </summary>
        private static bool _programStopping;

        /// <summary>
        /// Constructor
        /// </summary>
        static DriverClock()
        {
            //CrestronEnvironment.ProgramStatusEventHandler += ProgramStatusEventHandler;
        }

        /// <summary>
        /// Handles program events.
        /// This only monitors the program stopping.
        /// </summary>
        /// <param name="programEventType">The new program state.</param>
        /*internal static void ProgramStatusEventHandler(eProgramStatusEventType programEventType)
        {
            if (programEventType == eProgramStatusEventType.Stopping)
                _programStopping = true;
        }*/

        /// <summary>
        /// Callback for the static CTimer instance that will only invoke this once.
        /// This will sit in a perpetual loop depending on the system state and will exit when the program is stopping.
        /// <para>No drivers loaded: Waits <see cref="IdleClock5000ms"/> between loops.</para>
        /// <para>One or more drivers loaded: Waits <see cref="Clock25ms"/> between loops.</para>
        /// <para>Program is stopping: Exits callback and no more loops execute.</para>
        /// </summary>
        /// <param name="notUsed">Not used.</param>
        internal static void Driver25msClockCallback(object notUsed)
        {
            for (; ; )
            {
                if (_programStopping)
                {
                    //CrestronEnvironment.ProgramStatusEventHandler -= ProgramStatusEventHandler;
                    return;
                }

                if (Driver25msClockEventHandler == null)
                {
                    // No drivers are listening, sleep and check back later if there are drivers listening.
                    Thread.Sleep((int)IdleClock5000ms);
                }
                else
                {
                    try
                    {
                        Driver25msClockEventHandler();
                    }
                    catch (Exception e)
                    {
                        // Printing to error log for now since this exception should not happen very often.
                        // CCDRV-2586 covers updating this at a later point to use a static logger.
                        Console.WriteLine($"Notice: Crestron.RAD.Common.BasicDriver.DriverClock.Driver25msClockCallback encountered an exception: {e}");
                    }

                    // Repeat everything in ~25ms since there are listeners
                    Thread.Sleep((int)Clock25ms);
                }
            }
        }
    }
}