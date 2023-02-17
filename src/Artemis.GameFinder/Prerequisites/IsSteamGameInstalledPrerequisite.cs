using System.Runtime.InteropServices;
using Artemis.Core;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;

namespace Artemis.GameFinder.Prerequisites;

/// <summary>
///   Checks if a Steam game is installed
/// </summary>
public class IsSteamGameInstalledPrerequisite : PluginPrerequisite
{
    private readonly SteamHandler _steamHandler;
    public IsSteamGameInstalledPrerequisite(int gameId, string? gameName = null)
    {
        var gameNameOrId = gameName ?? gameId.ToString();
        InstallActions = new()
        {
            new RunInlinePowerShellAction($"Install game {gameNameOrId}", $"start \"steam://run/{gameId}\"")
        };
        UninstallActions = new();
        GameId = gameId;
        Name = $"Steam game \"{gameNameOrId}\" installed";
        Description = $"Steam game {gameNameOrId} must be installed to use this plugin";
        
        _steamHandler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new SteamHandler(new WindowsRegistry())
            : new SteamHandler(registry: null);
    }
    
    public override bool IsMet()
    {
        try
        {
            var games = _steamHandler.FindAllGames();
            return games.Any(game => game.Game?.AppId == GameId);
        }
        catch
        {
            return false;
        }
    }

    public int GameId { get; }
    public override string Name { get; }
    public override string Description { get; }
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }
}