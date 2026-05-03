using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Biometric;
using StoreIt.Maui.Services;
using StoreIt.Maui.ViewModels;
using StoreIt.Maui.Views;
using StoreIt.Navigation;
using StoreIt.Services;
using StoreIt.WhatsNew;
using ZXing.Net.Maui.Controls;

namespace StoreIt.Maui;

public static class MauiProgram
{
        public static MauiApp CreateMauiApp()
        {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .UseMauiCommunityToolkit()
                    .UseBarcodeReader()
                    .ConfigureFonts(fonts =>
                    {
                            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    })
                    ;

                // Services
                builder.Services.AddSingleton<DatabaseService>();
                builder.Services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
                builder.Services.AddSingleton<IThemeService, ThemeService>();
                builder.Services.AddSingleton<IPreferences>(Preferences.Default);
                builder.Services.AddSingleton<IAppNavigationService, ShellNavigationService>();
                builder.Services.AddSingleton<IDialogService, ShellDialogService>();
                builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);
                builder.Services.AddSingleton<IBiometricService, BiometricService>();
                builder.Services.AddSingleton<IWhatsNewService, WhatsNewService>();

                // Platform-specific services
#if ANDROID
                builder.Services.AddSingleton<IPlatformBrightnessService, Platforms.Android.AndroidBrightnessService>();
#elif IOS
        builder.Services.AddSingleton<IPlatformBrightnessService, Platforms.iOS.iOSBrightnessService>();
#elif MACCATALYST
        builder.Services.AddSingleton<IPlatformBrightnessService, Platforms.MacCatalyst.MacCatalystBrightnessService>();
#else
        builder.Services.AddSingleton<IPlatformBrightnessService, Services.FallbackBrightnessService>();
#endif
                // ViewModels
                builder.Services.AddTransient<MainViewModel>();
                builder.Services.AddTransient<AddCardViewModel>();
                builder.Services.AddTransient<ViewCardViewModel>();
                builder.Services.AddTransient<ScanBarcodeViewModel>();
                builder.Services.AddTransient<ManualBarcodeViewModel>();
                builder.Services.AddTransient<SettingsViewModel>();
                builder.Services.AddTransient<WhatsNewViewModel>();

                // Pages
                builder.Services.AddTransient<MainPage>();
                builder.Services.AddTransient<AddCardPage>();
                builder.Services.AddTransient<ViewCardPage>();
                builder.Services.AddTransient<ScanBarcodePage>();
                builder.Services.AddTransient<ManualBarcodePage>();
                builder.Services.AddTransient<SettingsPage>();

#if DEBUG
                builder.Logging.AddDebug();
#endif

                return builder.Build();
        }
}
