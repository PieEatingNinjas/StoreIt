# StoreIt 🗂️

A cross-platform mobile app for storing and managing your loyalty cards, membership cards, and access codes — all in one place.

Built with .NET MAUI, targeting **Android** and **iOS**.

---

## Features

- 📱 **Barcode support** — Scan or manually enter barcodes from existing cards
- 🔢 **Custom codes** — Store membership numbers, access codes, and any other identifiers
- 🎨 **Color labels** — Assign a color to each card for easy visual identification
- ⭐ **Favorites** — Mark frequently used cards as favorites so they appear at the top
- **Card details** — Fullscreen Shell page with native back, favorite, and edit actions; deletion stays in the overflow menu with confirmation. A centered card header and generous code panel keep the focus on scanning. Barcode zoom, copyable codes, biometric protection, and updated details after editing remain available.
- 🔐 **Private cards** — Protect sensitive cards with biometric authentication (Face ID / fingerprint)
- 🌙 **Dark & light theme** — Follows your system theme or lets you pick manually
- **Android system bars** — Status, navigation, and tab bars match the page background in light and dark mode, with contrasting system icons; transparent native app-bar backgrounds prevent a purple strip behind the status bar on edge-to-edge devices
- **Neutral action icons** — Thin anthracite outlines in light mode and white outlines in dark mode keep blue accents reserved for selection and focus
- **Calm secondary palette** — Shared blue-gray surfaces, selections, and secondary actions in both themes, with neutral gray input fields; card colors and high-contrast barcode displays stay unchanged
- **Consistent toggle colors** — Explicit light/dark track and thumb colors for on, off, and disabled states avoid manufacturer-dependent Android theme colors
- 💡 **Hints** — On-screen guidance that can be turned off once you're familiar with the app

---

## Screenshots

> _Coming soon_

---

## Tech Stack

| Dependency | Purpose |
|---|---|
| [.NET 10 MAUI](https://learn.microsoft.com/en-us/dotnet/maui/) | Cross-platform UI framework |
| [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui) | MAUI helpers and converters |
| [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) | MVVM source generators |
| [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) | Local SQLite database |
| [ZXing.Net.Maui](https://github.com/Redth/ZXing.Net.Maui) | Barcode scanning and generation |
| [Plugin.Maui.Biometric](https://github.com/oscoreio/Maui.Biometric) | Biometric authentication |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- .NET MAUI workload:
  ```bash
  dotnet workload install maui
  ```
  
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

### Card detail validation

```bash
dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net10.0-android -warnaserror
dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net10.0-ios27.1 -p:RuntimeIdentifier=iossimulator-arm64 -warnaserror
dotnet test tests/StoreIt.Maui.Tests/StoreIt.Maui.Tests.csproj
```

On both Android and iOS, check a barcode and a text-code card in light/dark mode:

- Open from the overview: a fullscreen page shows native back, favorite, and edit actions.
- Toggle favorite, go back, and check the overview ordering. Edit/save and edit/back should return to updated details.
- Cancel deletion, then confirm it; the overview must refresh without navigating away.
- Tap to zoom a barcode (including on a narrow screen), or copy a text code.
- Navigate back or edit the card: brightness and screen sleep return to normal.
- Open a private card, reject authentication, and dismiss during authentication: no card data is exposed.

Card details use the existing Shell navigation and MAUI controls, without a bottom-sheet dependency
or custom presentation/reopening flow.

---

## License

This project is licensed under the [MIT License](LICENSE).
