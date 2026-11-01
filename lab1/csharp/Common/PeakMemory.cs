using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Lab1.Common;

// Peak memory: the most memory the program used at any moment, in MB.
// On Mac and Linux we ask the operating system with getrusage(),
// because .NET's own value is not reliable on Mac.
public static class PeakMemory
{
    // The data getrusage() fills in. We only use MaxRss (the peak memory).
    [StructLayout(LayoutKind.Sequential)]
    private struct TimeVal { public long Sec; public long Usec; }

    [StructLayout(LayoutKind.Sequential)]
    private struct RUsage
    {
        public TimeVal UTime, STime;
        public long MaxRss, IxRss, IdRss, IsRss, MinFlt, MajFlt, NSwap,
                    InBlock, OuBlock, MsgSnd, MsgRcv, NSignals, NvCsw, NivCsw;
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int getrusage(int who, out RUsage usage);

    public static double Megabytes()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            if (getrusage(0, out var u) == 0) // 0 = this process
            {
                // Mac gives bytes, Linux gives kilobytes.
                double bytes = OperatingSystem.IsMacOS() ? u.MaxRss : u.MaxRss * 1024.0;
                return bytes / (1024 * 1024);
            }
        }
        // Windows
        return Process.GetCurrentProcess().PeakWorkingSet64 / (1024.0 * 1024);
    }
}
