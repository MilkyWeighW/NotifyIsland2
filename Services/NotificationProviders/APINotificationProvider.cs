using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using cn.lixiaotuan.notifyisland2.Services.NotificationAPIServer;

namespace cn.lixiaotuan.notifyisland2.Services.NotificationProviders;

[NotificationProviderInfo("534B80F5-8775-A978-95A3-F6A7BC5A1166", "API提醒", "通过HTTP接口触发的提醒。")]
public class APINotificationProvider : NotificationProviderBase
{
    public Plugin Plugin { get; }

    private CancellationTokenSource? _debounceCts;

    public APINotificationProvider(Plugin plugin)
    {
        Plugin = plugin;
        if (Plugin.Settings.Enabled) RestartServer();
        Plugin.Settings.PropertyChanged += OnSettingsChanged;
    }

    private async void OnSettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;
        try { await Task.Delay(500, token); }
        catch (TaskCanceledException) { return; }

        if (Plugin.Settings.Enabled) RestartServer();
        else StopServer();
    }

    private void RestartServer()
    {
        StopServer();
        if (string.IsNullOrEmpty(Plugin.Settings.Host)) return;
        if (Plugin.Settings.Port is < 1 or > 65535) return;
        if (!Plugin.Settings.Enabled) return;

        var url = $"http://{Plugin.Settings.Host}:{Plugin.Settings.Port}/";
        var server = new NotificationAPIServer.NotificationAPIServer(url, Plugin.Settings.Token);
        NotificationAPIServer.NotificationAPIServer.Current = server;
        server.NotificationReceived += OnNotificationReceived;
    }

    private void StopServer()
    {
        if (NotificationAPIServer.NotificationAPIServer.Current != null)
        {
            NotificationAPIServer.NotificationAPIServer.Current.NotificationReceived -= OnNotificationReceived;
            NotificationAPIServer.NotificationAPIServer.Current.Dispose();
        }
    }

    private void OnNotificationReceived(object? sender, NotificationReceivedEventArgs e)
    {
        var n = e.Notification;
        ShowNotification(new NotificationRequest
        {
            MaskContent = NotificationContent.CreateTwoIconsMask(
                n.title,
                hasRightIcon: false,
                factory: c =>
                {
                    c.Duration = TimeSpan.FromSeconds(n.title_duration);
                    c.SpeechContent = n.title_voice ?? n.title;
                    c.IsSpeechEnabled = true;
                }),
            OverlayContent = NotificationContent.CreateSimpleTextContent(n.content, c =>
            {
                c.Duration = TimeSpan.FromSeconds(n.content_duration);
                c.SpeechContent = n.content_voice ?? n.content;
                c.IsSpeechEnabled = true;
            }),
        });
    }
}
