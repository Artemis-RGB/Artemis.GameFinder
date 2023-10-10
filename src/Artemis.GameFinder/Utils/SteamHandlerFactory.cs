using System.Runtime.InteropServices;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;
using NexusMods.Paths;

namespace Artemis.GameFinder.Utils;

internal static class SteamHandlerFactory
{
    public static SteamHandler Create()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new SteamHandler(FileSystem.Shared, WindowsRegistry.Shared)
            : new SteamHandler(FileSystem.Shared, null);
    }
}