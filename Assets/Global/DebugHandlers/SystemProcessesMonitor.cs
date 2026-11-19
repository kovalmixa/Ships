using System;

namespace Assets.Global.DebugHandlers
{
    public static class SystemProcessesMonitor
    {
        public static float GetTotalAllocatedMemoryMB() => GC.GetTotalMemory(false) / (1024f * 1024f);
    }
}
