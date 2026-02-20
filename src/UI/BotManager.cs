using Bot.Helpers;

namespace Bot.UI;

public class BotManager
{
    private static Task? _botTask;

    public static CancellationTokenSource? Cts { get; private set; }

    public static async Task LoginAsync()
    {
        if (SEGBot.Client is not null && Cts is not null) return;

        Cts = new();
        
        await SEGBot.LoginAsync(Cts);
    }
    
    public static async Task<int> StartWait()
    {
        if (SEGBot.IsHeadless)
            Logger.OutputLogToStandardOut();

        Cts = new CancellationTokenSource();

        _botTask = SEGBot.RunAsync(Cts);
        await _botTask;
        return 0;
    }
    
    public static void Stop()
    {
        if (SEGBot.Client is null && Cts is null) return;
        
        Cts!.Cancel();
        _botTask = null;

        Cts = null;
    }
    
    public static string GetConnectionState()
        => SEGBot.Client is null
            ? "Disconnected"
            : Enum.GetName(SEGBot.Client.ConnectionState) ?? "Disconnected";
}