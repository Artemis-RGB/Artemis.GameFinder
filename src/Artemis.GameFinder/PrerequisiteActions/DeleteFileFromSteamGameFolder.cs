﻿using Artemis.Core;
using Artemis.GameFinder.Utils;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;

namespace Artemis.GameFinder.PrerequisiteActions;

public class DeleteFileFromSteamGameFolder : PluginPrerequisiteAction
{
    /// <summary>
    ///   Deletes a file from the specified steam game folder
    /// </summary>
    /// <param name="name">The name of the action</param>
    /// <param name="steamId">The id of the steam game</param>
    /// <param name="filePath">The path to the file to delete, relative to the game root.</param>
    public DeleteFileFromSteamGameFolder(string name, uint steamId, string filePath)
        : base(name)
    {
        SteamId = steamId;
        FilePath = filePath;
        ProgressIndeterminate = true;
    }

    public uint SteamId { get; }
    public string FilePath { get; }

    public override Task Execute(CancellationToken cancellationToken)
    {
        var steamHandler = SteamHandlerFactory.Create();

        var game = steamHandler.FindOneGameById(AppId.From(SteamId), out var errors);
        if (game == null)
            throw new ArtemisPluginException("Could not find game with id " + SteamId);

        var filePath = Path.Combine(game.Path.GetFullPath(), FilePath);
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}