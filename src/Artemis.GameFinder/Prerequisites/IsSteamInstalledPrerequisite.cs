using System.Runtime.InteropServices;
using Artemis.Core;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;

namespace Artemis.GameFinder.Prerequisites;

/// <summary>
///   Checks if Steam is installed
/// </summary>
public class IsSteamInstalledPrerequisite :  PluginPrerequisite
{
    public IsSteamInstalledPrerequisite(Plugin plugin)
    {
        var installPath = plugin.ResolveRelativePath("SteamSetup.exe");
        InstallActions = new()
        {
             new DownloadFileAction("Download Steam", "https://cdn.akamai.steamstatic.com/client/installer/SteamSetup.exe", installPath),
             new ExecuteFileAction("Install Steam", installPath, "/S"),
             new DeleteFileAction("Delete Steam installer", installPath)
        };
        UninstallActions = new();
        Name = "Steam installed";
        Description = "Steam must be installed to use this plugin";    
    }
    
    public override bool IsMet()
    {
        var handler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new SteamHandler(new WindowsRegistry())
            : new SteamHandler(registry: null);
        try
        {
            var games = handler.FindAllGames().ToList();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override string Name { get; }
    public override string Description { get; }
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }
}