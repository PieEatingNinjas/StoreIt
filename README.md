# StoreIt 🗂️

A cross-platform mobile app for storing and managing your loyalty cards, membership cards, and access codes — all in one place.

Built with .NET MAUI, targeting **Android** and **iOS**.

---

## Features

- 📱 **Barcode support** — Scan or manually enter barcodes from existing cards
- 🔢 **Custom codes** — Store membership numbers, access codes, and any other identifiers
- 🎨 **Color labels** — Assign a color to each card for easy visual identification
- ⭐ **Favorites** — Mark frequently used cards as favorites so they appear at the top
- **Card details** — Animated modal with toolbar actions for closing, favorites, and editing; deletion is available in the overflow menu
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

---

## License

This project is licensed under the [MIT License](LICENSE).
