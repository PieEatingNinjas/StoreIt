using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StoreIt.Maui.Services;
using StoreIt.Services;

namespace StoreIt.Maui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IAppNavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IWhatsNewService _whatsNewService;

    [ObservableProperty]
    private ThemeOption selectedTheme;

    [ObservableProperty]
    private bool hintsEnabled;

    public SettingsViewModel(IThemeService themeService,
        IUserPreferencesService userPreferencesService,
        IAppNavigationService appNavigationService, IWhatsNewService whatsNewService)
    {
        _navigationService = appNavigationService;
        _themeService = themeService;
        _userPreferencesService = userPreferencesService;
        _whatsNewService = whatsNewService;

        // Load current theme
        SelectedTheme = _themeService.GetCurrentTheme();

        // Load hints setting
        HintsEnabled = _userPreferencesService.GetHintsEnabled();

        // Listen for theme changes
        WeakReferenceMessenger.Default.Register<ThemeChangedMessage>(this, (_, _) => OnThemeChanged());
    }

    partial void OnHintsEnabledChanged(bool value)
        => _userPreferencesService.SetHintsEnabled(value);

    private void OnThemeChanged()
    {
        OnPropertyChanged(nameof(IsSystemTheme));
        OnPropertyChanged(nameof(IsLightTheme));
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(SelectedTheme));
    }

    [RelayCommand]
    private Task GoBack() => _navigationService.GoBack();

    public bool IsSystemTheme
    {
        get => SelectedTheme == ThemeOption.System;
        set
        {
            if (value) SelectedTheme = ThemeOption.System;
        }
    }

    public bool IsLightTheme
    {
        get => SelectedTheme == ThemeOption.Light;
        set
        {
            if (value) SelectedTheme = ThemeOption.Light;
        }
    }

    public bool IsDarkTheme
    {
        get => SelectedTheme == ThemeOption.Dark;
        set
        {
            if (value) SelectedTheme = ThemeOption.Dark;
        }
    }

    [RelayCommand]
    private void ThemeSelected(ThemeOption theme)
    {
        SelectedTheme = theme;
        _themeService.SetTheme(theme);
    }

    [RelayCommand]
    private async Task OpenWhatsNew()
    {
        await _whatsNewService.ShowLatestWhatsNewAsync(bypassCheck: true);
    }
}