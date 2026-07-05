using System.IO;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using cn.lixiaotuan.notifyisland2.Models;
using cn.lixiaotuan.notifyisland2.Services.NotificationProviders;
using cn.lixiaotuan.notifyisland2.Views;

namespace cn.lixiaotuan.notifyisland2;

[PluginEntrance]
public class Plugin : PluginBase
{
    public Settings Settings { get; set; } = new();

    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Settings = ConfigureFileHelper.LoadConfig<Settings>(Path.Combine(PluginConfigFolder, "settings.json"));
        Settings.PropertyChanged += (s, e) =>
        {
            ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(PluginConfigFolder, "settings.json"), Settings);
        };
        services.AddNotificationProvider<APINotificationProvider>();
        services.AddSettingsPage<NotifyIslandSettingsPage>();
    }
}
