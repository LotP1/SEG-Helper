using CommunityToolkit.Mvvm.ComponentModel;

namespace Bot.UI.Avalonia.Pages;

// ReSharper disable once InconsistentNaming
public partial class UIShellViewModel : ObservableObject
{
    public required UIShellView View { get; init; } 
    
    [ObservableProperty] private string _connection = BotManager.GetConnectionState();

    public UIShellViewModel()
    {
        SEGBot.Client.Connected += ChangeConnectionState;
        SEGBot.Client.Disconnected += Disconnected;
    }

    ~UIShellViewModel()
    {
        SEGBot.Client.Connected -= ChangeConnectionState;
        SEGBot.Client.Disconnected -= Disconnected;
    }

    private Task ChangeConnectionState()
    {
        Connection = BotManager.GetConnectionState();
        return Task.CompletedTask;
    }
    
    private Task Disconnected(Exception e)
    {
        SEGBotApp.NotifyError(e);
        return ChangeConnectionState();
    }
}