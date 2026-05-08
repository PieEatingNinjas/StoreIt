# StoreIt 🗂️

A cross-platform mobile app for storing and managing your loyalty cards, membership cards, and access codes — all in one place.

Built with .NET MAUI, targeting **Android** and **iOS**.

---

## Features

- 📱 **Barcode support** — Scan or manually enter barcodes from existing cards
- 🔢 **Custom codes** — Store membership numbers, access codes, and any other identifiers
- 🎨 **Color labels** — Assign a color to each card for easy visual identification
- ⭐ **Favorites** — Mark frequently used cards as favorites so they appear at the top
- 🔐 **Private cards** — Protect sensitive cards with biometric authentication (Face ID / fingerprint)
- 🌙 **Dark & light theme** — Follows your system theme or lets you pick manually
- 💡 **Hints** — On-screen guidance that can be turned off once you're familiar with the app

---

## Screenshots

> _Coming soon_

---

## Tech Stack

| Dependency | Purpose |
|---|---|
| [.NET 9 MAUI](https://learn.microsoft.com/en-us/dotnet/maui/) | Cross-platform UI framework |
| [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui) | MAUI helpers and converters |
| [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) | MVVM source generators |
| [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) | Local SQLite database |
| [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) | Barcode scanning and generation |
| [Plugin.Maui.Biometric](https://github.com/oscoreio/Maui.Biometric) | Biometric authentication |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI workload:
  ```bash
  dotnet workload install maui
  ```
- **Android**: Android SDK (API level 21+)
- **iOS**: macOS with Xcode 16+

### Build

```bash
# Clone the repository
git clone https://github.com/PieEatingNinjas/StoreIt.git
cd StoreIt

# Restore dependencies
dotnet restore src/StoreIt.Maui/StoreIt.Maui.csproj

# Build for Android
dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net9.0-android

# Build for iOS (macOS only)
dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net9.0-ios
```

### Run

Use Visual Studio on Windows (with a Mac build host for iOS), the [.NET MAUI VS Code extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui), or the `dotnet` CLI to deploy to a physical device or emulator.

---

## Project Structure

```
src/
└── StoreIt.Maui/
    ├── Controls/         # Reusable UI controls
    ├── Converters/       # Value converters for data binding
    ├── Helpers/          # Utility classes
    ├── Models/           # Data models
    ├── Navigation/       # Navigation service abstractions
    ├── Platforms/        # Platform-specific implementations
    ├── Resources/        # Fonts, images, styles
    ├── Services/         # Business logic and services
    ├── ViewModels/       # MVVM view models
    ├── Views/            # XAML pages
    └── WhatsNew/         # What's New feature pages
```

---

## Contributing

Contributions are welcome! Please read the [Contributing Guide](CONTRIBUTING.md) before submitting a pull request.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

_Built with ❤️ in Belgium_
