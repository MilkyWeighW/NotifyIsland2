using CommunityToolkit.Mvvm.ComponentModel;

namespace cn.lixiaotuan.notifyisland2_BatchRan.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty] private string _host = "localhost";
    [ObservableProperty] private int _port = 1380;
    [ObservableProperty] private string _token = "";
    [ObservableProperty] private bool _enabled = false;
}
