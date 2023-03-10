using System.Runtime.InteropServices;
using Artemis.Core;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;

namespace Artemis.GameFinder.PrerequisiteActions;

public class DeleteFileFromSteamGameFolder : PluginPrerequisiteAction
{
    /// <summary>
    ///   Deletes a file from the specified steam game folder
    /// </summary>
    /// <param name="name">The name of the action</param>
    /// <param name="steamId">The id of the steam game</param>
    /// <param name="filePath">The path to the file to delete, relative to the game root.</param>
    public DeleteFileFromSteamGameFolder(string name, int steamId, string filePath)
        : base(name)
    {
        SteamId = steamId;
        FilePath = filePath;
        ProgressIndeterminate = true;
    }
    
    public int SteamId { get; }
    public string FilePath { get; }
    
    public override Task Execute(CancellationToken cancellationToken)
    {
        var steamHandler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new SteamHandler(new WindowsRegistry())
            : new SteamHandler(registry: null);

        var game = steamHandler.FindOneGameById(SteamId, out var errors);
        if (game == null)
            throw new ArtemisPluginException("Could not find game with id " + SteamId);

        var filePath = Path.Combine(game.Path, FilePath);
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}