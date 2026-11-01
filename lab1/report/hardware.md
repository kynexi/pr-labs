# Hardware (measurement machine)

All official measurements are taken on this machine only.

| Item | Value |
|---|---|
| Laptop model | Lenovo 83D3 |
| CPU model | AMD Ryzen 7 8845HS with Radeon 780M Graphics |
| Physical cores | 8 |
| Logical processors | 16 |
| RAM | 27.8 GiB available to Windows (32 GB installed) |
| Internal disk | WD PC SN740 SDDPMQD-1T00-1101, 1 TB NVMe SSD |
| USB drive | Kingston DataTraveler 3.0, 64 GB nominal capacity |
| USB version (drive / port) | USB 3.0 drive (model); connected through an AMD USB 3.10 xHCI host controller. Negotiated link speed was not confirmed. |
| Operating system | Windows 11 Pro, build 26200 |
| Language / runtime version | C# / .NET SDK 8.0.412 |

These are the values reported by Windows on the measurement machine. Windows
reports the USB drive as removable media on the USB bus and identifies its
model as `Kingston DataTraveler 3.0`; it does not expose the negotiated USB
link speed in the device information collected for this report.

PowerShell commands used to collect system details:

```powershell
Get-CimInstance Win32_Processor |
  Select-Object Name, NumberOfCores, NumberOfLogicalProcessors
Get-CimInstance Win32_ComputerSystem |
  Select-Object Model, @{Name='RAM_GiB';Expression={[math]::Round($_.TotalPhysicalMemory / 1GB, 1)}}
Get-CimInstance Win32_OperatingSystem |
  Select-Object Caption, Version
Get-CimInstance Win32_DiskDrive |
  Select-Object Model, MediaType, InterfaceType, Size
dotnet --version
```
