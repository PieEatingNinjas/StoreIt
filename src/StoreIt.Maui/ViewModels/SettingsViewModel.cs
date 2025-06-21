using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoreIt.Maui.Services;

namespace StoreIt.Maui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IThemeService _themeService;
    private readonly IUserPreferencesService _userPreferencesService;

    [ObservableProperty]
    private ThemeOption selectedTheme;
    
    [ObservableProperty]
    private bool hintsEnabled;

    public SettingsViewModel(IThemeService themeService, IUserPreferencesService userPreferencesService)
    {
        _themeService = themeService;
        _userPreferencesService = userPreferencesService;

        // Load current theme
        SelectedTheme = _themeService.GetCurrentTheme();
        
        // Load hints setting
        HintsEnabled = _userPreferencesService.GetHintsEnabled();

        // Listen for theme changes
        _themeService.ThemeChanged += OnThemeChanged;
    }

    partial void OnSelectedThemeChanged(ThemeOption value)
    {
        _themeService.SetTheme(value);
    }
    
    partial void OnHintsEnabledChanged(bool value)
    {
        _userPreferencesService.SetHintsEnabled(value);
    }

    private void OnThemeChanged(object? sender, ThemeOption theme)
    {
        OnPropertyChanged(nameof(IsSystemTheme));
        OnPropertyChanged(nameof(IsLightTheme));
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(SelectedTheme));
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public bool IsSystemTheme
    {
        get => SelectedTheme == ThemeOption.System;
        set { if (value) SelectedTheme = ThemeOption.System; }
    }

    public bool IsLightTheme
    {
        get => SelectedTheme == ThemeOption.Light;
        set { if (value) SelectedTheme = ThemeOption.Light; }
    }

    public bool IsDarkTheme
    {
        get => SelectedTheme == ThemeOption.Dark;
        set { if (value) SelectedTheme = ThemeOption.Dark; }
    }
}
