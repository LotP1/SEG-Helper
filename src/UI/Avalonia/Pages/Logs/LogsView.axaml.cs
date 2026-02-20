using Avalonia.Controls;
using Bot.Helpers;
using Bot.UI.Helpers;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;

namespace Bot.UI.Avalonia.Pages;

[UiPage(PageType.Logs, "Bot Logs", Symbol.AllApps, isDefault: true, isFooter: true)]
public partial class LogsView : UserControl
{
    public LogsView()
    {
        InitializeComponent();
        DataContext = new LogsViewModel { View = this, LogsClearAmount = 10 };

        CopySimpleIcon.Value = FontAwesome.Copy;
        CopySimple.Command = new AsyncRelayCommand(async () =>
        {
            if (this.Context<LogsViewModel>().Selected is { } selected)
                await OS.CopyToClipboardAsync(selected.FormattedString);
        });

        CopyMarkdownIcon.Value = FontAwesome.Brush;
        CopyMarkdown.Command = new AsyncRelayCommand(async () =>
        {
            if (this.Context<LogsViewModel>().Selected is { } selected)
                await OS.CopyToClipboardAsync(selected.Markdown);
        });
    }
}