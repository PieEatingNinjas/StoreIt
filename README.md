<p align="center">
  <img src="docs/assets/storeit-logo.svg" alt="StoreIt logo" width="120" height="120">
</p>

# StoreIt

A cross-platform mobile app for storing and managing your loyalty cards, membership cards, and access codes — all in one place.

Built with **.NET 10 MAUI** for **Android** and **iOS**. StoreIt is a frontend-only,
offline app: data stays on-device in SQLite, with no remote APIs or cloud backend.

For app information and download links, visit the [StoreIt website](https://pieeatingninjas.github.io/StoreIt/).

Requires **Android 7.0 (API 24)** or newer and **iOS 15** or newer.

## Features

- Scan barcodes or store cards and codes manually.
- Organize cards with favorites, sorting, and color labels.
- Protect private cards with biometric authentication.
- View barcodes and copy text codes.
- Light and dark themes.

## Tech Stack

| Dependency | Purpose |
|---|---|
| [.NET 10 MAUI](https://learn.microsoft.com/en-us/dotnet/maui/) | Cross-platform UI framework |
| [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui) | MAUI helpers and converters |
| [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) | MVVM source generators |
| [DuoExtensions.Maui](https://www.nuget.org/packages/DuoExtensions.Maui) | Platform-aware toolbar item behavior |
| [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) | Local SQLite database |
| [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) | Barcode scanning and generation |
| [Plugin.Maui.Biometric](https://github.com/oscoreio/Maui.Biometric) | Biometric authentication |
| [Plugin.Maui.BottomSheet](https://github.com/lucacivale/Maui.BottomSheet) | Native card-detail bottom sheets on iOS and Android |

## Architecture

- **MVVM:** XAML views with compiled bindings; observable state and commands use CommunityToolkit.Mvvm.
- **Dependency injection:** ViewModels, services, and navigation are registered in [MauiProgram.cs](src/StoreIt.Maui/MauiProgram.cs).
- **Local persistence:** SQLite stores cards on-device; platform services handle biometrics and device behavior.
- **Navigation:** Shell hosts pages; native bottom sheets present card details above the overview.

Package versions and target frameworks are defined in [StoreIt.Maui.csproj](src/StoreIt.Maui/StoreIt.Maui.csproj).

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- .NET MAUI workload:
  ```bash
  dotnet workload install maui
  ```
- Android SDK and JDK 17 for Android builds.
- A Mac with Xcode 27.2 for the current iOS target.

### Build and test

Run from the repository root:

```bash
dotnet restore src/StoreIt.Maui/StoreIt.Maui.csproj
dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net10.0-android -warnaserror
dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net10.0-ios27.2 -p:RuntimeIdentifier=iossimulator-arm64 -warnaserror
dotnet test tests/StoreIt.Maui.Tests/StoreIt.Maui.Tests.csproj
```

The iOS simulator command targets Apple Silicon; use `iossimulator-x64` on Intel Macs.
Debug builds otherwise use the SDK's host-appropriate simulator architecture.
Release builds default to `ios-arm64` for physical devices.

The xUnit project is currently a placeholder with no tests; device/simulator smoke
testing is still required. [GitHub Actions](.github/workflows/ci.yml) builds both platforms
and runs the test project.

## Project Structure

```
src/StoreIt.Maui/
├── Controls/         # Reusable UI controls
├── Converters/       # Binding converters
├── Models/           # Data models
├── Navigation/       # Navigation services and abstractions
├── Platforms/        # Android and iOS implementations
├── Resources/        # App icons, fonts, images, and styles
├── Services/         # Persistence and device services
├── Sorting/          # Card ordering logic
├── ViewModels/       # Observable state and commands
├── Views/            # XAML pages and bottom sheets
└── WhatsNew/         # Release highlights
tests/StoreIt.Maui.Tests/  # xUnit test project (placeholder)
docs/                    # GitHub Pages website
└── assets/              # Shared website and README logo
```

## Contributing

Contributions are welcome! Please read the [Contributing Guide](CONTRIBUTING.md) before submitting a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
