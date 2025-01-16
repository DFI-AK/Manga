using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Manga.Application.Common.Interfaces;
using Manga.Application.Common.Models;

namespace Manga.Infrastructure.Services;
internal sealed class SystemProcessingService() : ISystemProcessingService
{
    [SupportedOSPlatform("windows")]
    private readonly PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    [SupportedOSPlatform("windows")]
    private readonly PerformanceCounter _memoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
    [SupportedOSPlatform("windows")]
    private readonly PerformanceCounter _diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
    [SupportedOSPlatform("windows")]
    private readonly PerformanceCounter _diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

    [SupportedOSPlatform("windows")]
    public SystemDetailModel GetSystemDetail()
    {
        try
        {
            var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            long totalMemoryKB = 0, freeMemoryKB = 0;

            foreach (var os in searcher.Get())
            {
                totalMemoryKB = Convert.ToInt64(os["TotalVisibleMemorySize"]);
                freeMemoryKB = Convert.ToInt64(os["FreePhysicalMemory"]);
            }

            return new SystemDetailModel
            {
                MachineName = Environment.MachineName,
                OSVersion = RuntimeInformation.OSDescription,
                ProcessorCount = Environment.ProcessorCount,
                Architecture = RuntimeInformation.OSArchitecture.ToString(),
                TotalMemoryKB = totalMemoryKB,
                FreeMemoryKB = freeMemoryKB
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occured while getting the system details : {message}", ex.Message);
        }
        return new();
    }

    [SupportedOSPlatform("windows")]
    public (double CpuUsage, double MemoryUsage, long DiskRead, long DiskWrite) GetSystemUsage()
    {
        _cpuCounter.NextValue();
        _memoryCounter.NextValue();
        _diskReadCounter.NextValue();
        _diskWriteCounter.NextValue();

        Thread.Sleep(1000);

        return (
            CpuUsage: _cpuCounter.NextValue(),
            MemoryUsage: _memoryCounter.NextValue(),
            DiskRead: (long)_diskReadCounter.NextValue(),
            DiskWrite: (long)_diskWriteCounter.NextValue()
        );
    }

    [SupportedOSPlatform("windows")]
    public (long BytesSent, long BytesReceived) GetNetworkUsage()
    {
        var category = new PerformanceCounterCategory("Network Interface");
        var instanceNames = category.GetInstanceNames();

        long bytesSent = 0;
        long bytesReceived = 0;

        foreach (var name in instanceNames)
        {
            using var sentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
            using var receivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);

            // Warm-up call
            sentCounter.NextValue();
            receivedCounter.NextValue();
        }

        System.Threading.Thread.Sleep(1000); // Allow counters to stabilize

        foreach (var name in instanceNames)
        {
            using var sentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
            using var receivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);

            bytesSent += (long)sentCounter.NextValue();
            bytesReceived += (long)receivedCounter.NextValue();
        }

        return (BytesSent: bytesSent, BytesReceived: bytesReceived);
    }

}
