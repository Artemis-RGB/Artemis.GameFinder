using System.Runtime.InteropServices;
using Artemis.Core;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;

namespace Artemis.GameFinder;

public class CopyFileToSteamGameFolderAction : PluginPrerequisiteAction
{
    /// <summary>
    /// Copies a file to the specified steam game folder
    /// </summary>
    /// <param name="name">The name of the action</param>
    /// <param name="steamGameId">The steam game id</param>
    /// <param name="source">The absolute path to the file to copy</param>
    /// <param name="destination">The destination path of the file, relative to the game root.</param>
    public CopyFileToSteamGameFolderAction(string name, int steamGameId, string source, string destination) 
        : base(name)
    {
        SteamGameId = steamGameId;
        Source = source;
        Destination = destination;
        ProgressIndeterminate = true;
    }
    
    public int SteamGameId { get; }
    public string Source { get; }
    public string Destination { get; }
    
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var steamHandler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new SteamHandler(new WindowsRegistry())
            : new SteamHandler(registry: null);
        
        var game = steamHandler.FindOneGameById(SteamGameId, out var errors);
        if (game == null)
            throw new ArtemisPluginException("Could not find game with id " + SteamGameId);
        
        var destinationPath = Path.Combine(game.Path, Destination);

        await using var source = File.Open(Source, FileMode.Open);
        await using var destination = File.Create(destinationPath);
        
        await source.CopyToAsync(destination, cancellationToken);
    }
}