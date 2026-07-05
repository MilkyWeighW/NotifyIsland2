using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;

namespace cn.lixiaotuan.notifyisland2.Views;

[SettingsPageInfo("notifyisland.settingspage", "NotifyIsland2", category: SettingsPageCategory.Debug)]
public partial class NotifyIslandSettingsPage : SettingsPageBase
{
    public Plugin Plugin { get; }

    public NotifyIslandSettingsPage() : this(null!)
    {
    }

    public NotifyIslandSettingsPage(Plugin plugin)
    {
        Plugin = plugin;
        InitializeComponent();
        DataContext = this;
    }
}
