using System;


namespace Crestron.RAD.Common.BasicDriver.FakeFeedback
{
    internal sealed class FakeFeedbackData
    {
        internal int StartTick { get; private set; }
        internal bool ForceRemoval { get; set; }

        internal FakeFeedbackData()
        {
            StartTick = Environment.TickCount;
        }
    }

}