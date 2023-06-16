using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Artemis.Core;
using Artemis.GameFinder.PrerequisiteActions;
using Artemis.GameFinder.Utils;
using DryIoc.ImTools;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;

namespace Artemis.GameFinder.Prerequisites;

public class IsFilePresentInSteamGameFolderPrerequisite : PluginPrerequisite
{
    private readonly SteamHandler _steamHandler;
    private readonly string _destinationPathRelative;
    private readonly string _sourcePath;
    private readonly int _gameId;

    public IsFilePresentInSteamGameFolderPrerequisite(int gameId, string sourcePath, string destinationPathRelative)
    {
        _gameId = gameId;
        _destinationPathRelative = destinationPathRelative;
        _sourcePath = sourcePath;
        _steamHandler = SteamHandlerFactory.Create();

        Name = $"File present in game folder";
        Description = $"File \"{destinationPathRelative}\" must be present in Steam game folder to use this plugin";
        InstallActions = new()
        {
            new CopyFileToSteamGameFolderAction(
                "Copy CS:GO GSI Config",
                _gameId, 
                _sourcePath,
                _destinationPathRelative)
        };

        UninstallActions = new()
        {
            new DeleteFileFromSteamGameFolder(
                "Delete CS:GO GSI Config",
                _gameId,
                _destinationPathRelative)
        };
    }
    
    public override string Name { get; }
    public override string Description { get; }
    public override List<PluginPrerequisiteAction> InstallActions { get; }
    public override List<PluginPrerequisiteAction> UninstallActions { get; }
    
    public override bool IsMet()
    {
        if (!TryGetGamePath(_gameId, out var path))
            return false;
        
        return File.Exists(Path.Combine(path, _destinationPathRelative));
    }
    
    private bool TryGetGamePath(int id, [NotNullWhen(true)] out string? path)
    {
        var games = _steamHandler.FindAllGames();
        var game = games.Where(x => x.IsT0)
                        .Select(r => r.AsT0)
                        .FirstOrDefault(g => g?.AppId == SteamGameId.From(id));
        
        if (game is null)
        {
            path = null;
            return false;
        }

        path = game.Path.GetFullPath();
        return true;
    }
}