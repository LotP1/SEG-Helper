using System.Text.Json;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Bot.Helpers;
using Bot.Interactions;
using Bot.Services;
using Gommon;
using MenuFactory.Abstractions.Attributes;

// ReSharper disable UnusedMember.Global
// These members are never directly invoked.

namespace Bot.UI.Avalonia.Pages;

// ReSharper disable once InconsistentNaming
public class ShellViewMenu
{
    [Menu("Clear Commands", "Dev", Icon = "fa-solid fa-broom")]
    public static async Task ClearCommands()
    {
        var interactionService = SEGBot.Services.Get<SEGBotInteractionService>();
        if (interactionService is null || SEGBot.Client is null)
        {
            SEGBotApp.Notify("Not logged in", "State error", NotificationType.Error);
            return;
        }

#if DEBUG
        var removedCommandsText = $"{await interactionService.ClearAllCommandsInGuildAsync(DiscordHelper.DevGuildId)} commands removed";
#else
        var removedCount = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var guildId in Config.WhitelistGuilds)
        {
            removedCount = Math.Max(await interactionService.ClearAllCommandsInGuildAsync(guildId), removedCount);
        }
        var removedCommandsText = $"{removedCount} commands removed from {Config.WhitelistGuilds.Count()} guilds.";
#endif

        SEGBotApp.Notify(removedCommandsText, "Interaction commands cleared");
    }

    [Menu("Export TitleDB", "Dev", Icon = "fa-solid fa-file-export")]
    public static async Task ExportTitleDb()
    {
        var pickedFile = await SEGBotApp.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Nintendo Switch Online RomFS TitleDB")
                {
                    Patterns = ["*.titlesdb"]
                }
            ]
        });

        if (pickedFile.None())
            return;

        var jdoc = JsonDocument.Parse(await File.ReadAllTextAsync(pickedFile[0].Path.AbsolutePath));

        Dictionary<string, List<string>> gameNamesToTitleIds = [];

        foreach (var jsonProperty in jdoc.RootElement.GetProperty("titles").EnumerateObject())
        {
            var title = jsonProperty.Value.GetProperty("title").GetString()!;
            
            if (!gameNamesToTitleIds.ContainsKey(title))
                gameNamesToTitleIds.Add(title, []);
            
            gameNamesToTitleIds[title].Add(jsonProperty.Name.Replace("-", "_"));
        }

        var fp = FilePath.Data / "titledb" / $"clean-{Path.GetFileName(pickedFile[0].Path.AbsolutePath)}";
            
        if (fp.TryGetParent(out var parent) && !parent.ExistsAsDirectory)
            Directory.CreateDirectory(parent.Path);
        
        fp.WriteAllLines(gameNamesToTitleIds
            .Select(kvp => 
                $"{kvp.Value.Select(x => $"\"{x.ToLower()}\"").JoinToString(" or ")} => Playing(\"{kvp.Key}\"),"
            ));
        
        SEGBotApp.Notify($"Exported TitleDB to {fp.Path}");
    }
}