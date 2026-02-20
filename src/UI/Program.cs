using Avalonia;
using Bot.Helpers;
using Bot.UI.Avalonia;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Logger = Bot.Helpers.Logger;

namespace Bot.UI;

public class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        if (!UnixHelper.TryParseNamedArguments(args, out var output) && output.Error is not InvalidOperationException)
            Logger.Error(output.Error);
        
        SEGBot.IsHeadless = args.Contains("--no-gui");

        if (SEGBot.IsHeadless) 
            return await BotManager.StartWait();

        await BotManager.LoginAsync();

        IconProvider.Current.Register<FontAwesomeIconProvider>();
        
        return BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<SEGBotApp>()
            .UsePlatformDetect()
            .WithInterFont();
}