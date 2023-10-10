using Artemis.GameFinder.Prerequisites;

namespace Artemis.GameFinder.Example;

public static class Program
{
    public static void Main(string[] args)
    {
        var steamInstalled = new IsSteamInstalledPrerequisite(null!);
        Console.WriteLine($"Steam installed: {steamInstalled.IsMet()}");

        var gameInstalled = new IsSteamGameInstalledPrerequisite(548430);
        Console.WriteLine($"Steam game installed: {gameInstalled.IsMet()}");

        var filePresent = new IsFilePresentInSteamGameFolderPrerequisite(548430, "nothing", "FSD.exe");
        Console.WriteLine($"File present: {filePresent.IsMet()}");
    }
}