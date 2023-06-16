using System.Runtime.InteropServices;
using Artemis.Core;
using Artemis.GameFinder.Utils;
using GameFinder.Common;
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
        
        _steamHandler = SteamHandlerFactory.Create();
    }
    
    public override bool IsMet()
    {
        try
        {
            var maybeGames = _steamHandler.FindAllGames();
            return maybeGames.Any(option => option.TryPickT0(out var game, out var _) && game.AppId == GameId);
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