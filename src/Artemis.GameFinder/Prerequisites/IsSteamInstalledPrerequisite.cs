using Artemis.Core;
using Artemis.GameFinder.Utils;

namespace Artemis.GameFinder.Prerequisites;

/// <summary>
///   Checks if Steam is installed
/// </summary>
public class IsSteamInstalledPrerequisite : PluginPrerequisite
{
    public IsSteamInstalledPrerequisite(Plugin plugin)
    {
        Name = "Steam installed";
        Description = "Steam must be installed to use this plugin";

        var installPath = plugin?.ResolveRelativePath("SteamSetup.exe") ?? "SteamSetup.exe";
        InstallActions = new List<PluginPrerequisiteAction>
        {
            new DownloadFileAction("Download Steam", "https://cdn.akamai.steamstatic.com/client/installer/SteamSetup.exe", installPath),
            new ExecuteFileAction("Install Steam", installPath, "/S"),
            new DeleteFileAction("Delete Steam installer", installPath)
        };
        UninstallActions = new List<PluginPrerequisiteAction>();
    }

    public override bool IsMet()
    {
        var handler = SteamHandlerFactory.Create();
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