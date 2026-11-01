# Hardware (measurement machine)

All official measurements are taken on this machine only.

| Item | Value |
|---|---|
| CPU model | |
| Physical cores | |
| Logical processors | |
| RAM | |
| Disk type | |
| USB drive (model, capacity) | |
| USB version (drive / port) | |
| Operating system | |
| Language / runtime version | C# / .NET … (`dotnet --version`) |

macOS commands:

```bash
sysctl -n machdep.cpu.brand_string   # CPU model
sysctl -n hw.physicalcpu hw.logicalcpu
sysctl -n hw.memsize                 # RAM in bytes
sw_vers                              # macOS version
system_profiler SPUSBDataType        # USB drive + speed (plug it in first)
dotnet --info
```
