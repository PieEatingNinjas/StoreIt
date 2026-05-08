using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Services;
using StoreIt.Services;

namespace StoreIt.WhatsNew;

public partial class WhatsNewViewModel : ObservableObject
{
    private readonly IUserPreferencesService _prefs;
    private readonly IAppNavigationService _navigationService;

    [ObservableProperty] private string version;

    [ObservableProperty] private string title;

    [ObservableProperty] private bool isHideForFutureVisible;

    public WhatsNewViewModel(IUserPreferencesService prefs, IAppNavigationService navigationService)
    {
        _prefs = prefs;
        _navigationService = navigationService;

        var latest = WhatsNewRegistry.Items.OrderByDescending(i => i.Id).First();
        Version = latest.Version ?? AppInfo.VersionString;
        Title = $"Nieuw in versie {Version}";
        isHideForFutureVisible = (_prefs.GetLastViewedWhatsNewId() ?? int.MinValue) < latest.Id;
    }

    [RelayCommand]
    private void Close()
    {
        _navigationService.GoBack();
    }

    [RelayCommand]
    private void HideForFuture()
    {
        _prefs.SetLastViewedWhatsNewId(WhatsNewRegistry.Items.OrderByDescending(i => i.Id).First().Id);
        Close();
    }
}