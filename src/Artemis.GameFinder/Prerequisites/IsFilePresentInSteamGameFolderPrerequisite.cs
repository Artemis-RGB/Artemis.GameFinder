using System.Diagnostics.CodeAnalysis;
using Artemis.Core;
using Artemis.GameFinder.PrerequisiteActions;
using Artemis.GameFinder.Utils;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace Artemis.GameFinder.Prerequisites;

public class IsFilePresentInSteamGameFolderPrerequisite : PluginPrerequisite
{
    private readonly SteamHandler _steamHandler;
    private readonly string _destinationPathRelative;
    private readonly string _sourcePath;
    private readonly uint _gameId;

    public IsFilePresentInSteamGameFolderPrerequisite(uint gameId, string sourcePath, string destinationPathRelative)
    {
        _gameId = gameId;
        _destinationPathRelative = destinationPathRelative;
        _sourcePath = sourcePath;
        _steamHandler = SteamHandlerFactory.Create();

        Name = $"File present in game folder";
        Description = $"File \"{destinationPathRelative}\" must be present in Steam game folder to use this plugin";
        InstallActions = new List<PluginPrerequisiteAction>
        {
            new CopyFileToSteamGameFolderAction(
                "Copy CS:GO GSI Config",
                _gameId,
                _sourcePath,
                _destinationPathRelative)
        };

        UninstallActions = new List<PluginPrerequisiteAction>
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

    private bool TryGetGamePath(uint id, [NotNullWhen(true)] out string? path)
    {
        var games = _steamHandler.FindAllGames();
        var game = games.Where(x => x.IsT0)
            .Select(r => r.AsT0)
            .FirstOrDefault(g => g?.AppId == AppId.From(id));

        if (game is null)
        {
            path = null;
            return false;
        }

        path = game.Path.GetFullPath();
        return true;
    }
}