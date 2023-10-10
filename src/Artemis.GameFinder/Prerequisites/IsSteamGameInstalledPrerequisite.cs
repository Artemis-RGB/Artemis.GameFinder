using Artemis.Core;
using Artemis.GameFinder.Utils;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace Artemis.GameFinder.Prerequisites;

/// <summary>
///   Checks if a Steam game is installed
/// </summary>
public class IsSteamGameInstalledPrerequisite : PluginPrerequisite
{
    private readonly SteamHandler _steamHandler;

    public IsSteamGameInstalledPrerequisite(uint gameId, string? gameName = null)
    {
        var gameNameOrId = gameName ?? gameId.ToString();
        InstallActions = new List<PluginPrerequisiteAction>
        {
            new RunInlinePowerShellAction($"Install game {gameNameOrId}", $"start \"steam://run/{gameId}\"")
        };
        UninstallActions = new List<PluginPrerequisiteAction>();
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
            return maybeGames.Any(option => option.TryPickT0(out var game, out var _) && game.AppId == AppId.From(GameId));
        }
        catch
        {
            return false;
        }
    }

    public uint GameId { get; }
    public override string Name { get; }
    public override string Description { get; }
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }
}