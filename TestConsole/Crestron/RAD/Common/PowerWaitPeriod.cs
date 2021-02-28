namespace Crestron.RAD.Common
{
    public class PowerWaitPeriod
    {
        public bool UseLocalWarmupTimer { get; set; }
        public uint WarmUpTime { get; set; }

        public bool UseLocalCooldownTimer { get; set; }
        public uint CoolDownTime { get; set; }
    }
}
