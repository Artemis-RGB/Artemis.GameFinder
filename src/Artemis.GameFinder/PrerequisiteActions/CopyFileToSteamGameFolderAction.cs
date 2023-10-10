using Artemis.Core;
using Artemis.GameFinder.Utils;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace Artemis.GameFinder.PrerequisiteActions;

public class CopyFileToSteamGameFolderAction : PluginPrerequisiteAction
{
    /// <summary>
    /// Copies a file to the specified steam game folder
    /// </summary>
    /// <param name="name">The name of the action</param>
    /// <param name="steamId">The steam game id</param>
    /// <param name="source">The absolute path to the file to copy</param>
    /// <param name="destination">The destination path of the file, relative to the game root.</param>
    public CopyFileToSteamGameFolderAction(string name, uint steamId, string source, string destination)
        : base(name)
    {
        SteamId = steamId;
        Source = source;
        Destination = destination;
        ProgressIndeterminate = true;
    }

    public uint SteamId { get; }
    public string Source { get; }
    public string Destination { get; }

    public override async Task Execute(CancellationToken cancellationToken)
    {
        var steamHandler = SteamHandlerFactory.Create();

        var game = steamHandler.FindOneGameById(AppId.From(SteamId), out var errors);
        if (game == null)
            throw new ArtemisPluginException("Could not find game with id " + SteamId);

        var destinationPath = Path.Combine(game.Path.GetFullPath(), Destination);

        await using var source = File.Open(Source, FileMode.Open);
        await using var destination = File.Create(destinationPath);

        await source.CopyToAsync(destination, cancellationToken);
    }
}