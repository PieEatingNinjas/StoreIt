# StoreIt 🗂️

A cross-platform mobile app for storing and managing your loyalty cards, membership cards, and access codes — all in one place.

Built with .NET MAUI, targeting **Android** and **iOS**.

---

## Features

- 📱 **Barcode support** — Scan or manually enter barcodes from existing cards
- 🔢 **Custom codes** — Store membership numbers, access codes, and any other identifiers
- 🎨 **Color labels** — Assign a color to each card for easy visual identification
- ⭐ **Favorites** — Mark frequently used cards as favorites so they appear at the top
- **Card details** — Native, draggable bottom sheet above the overview, opening at three-quarter height and expandable to full height, with a colored card header and a generous code panel. Inset header controls close on the left and toggle favorites on the right; styled edit and delete actions sit at the bottom, remaining scrollable on smaller screens. Deletion requires confirmation. Barcode zoom, copyable codes, biometric protection, and updated details after editing remain available.
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
| [Plugin.Maui.BottomSheet](https://github.com/lucacivale/Maui.BottomSheet) | Native card-detail bottom sheets on iOS and Android |

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

The xUnit project is currently a placeholder and contains no tests. Simulator smoke checks cover
text and QR-code sheets, light/dark mode, favorite updates, edit/save and edit/back, confirmation
display, overview refresh, and screen-sleep restoration. Deletion cancellation/confirmation were
also exercised with scripted dialog responses during local UI automation.

On both Android and iOS, check a barcode and a text-code card in light/dark mode:

- Open from the overview: the sheet starts at 75% of the window height, with inset close/favorite controls and edit/delete actions at the bottom. Drag up to full height and back down. On smaller screens or with larger text/code, scroll to reach every action. Rotate the device and check that the three-quarter height adapts.
- Toggle favorite in the header: its star and accessibility description update. Close with the left close icon, swipe, or Android back and check the overview ordering.
- Edit/save and edit/back should reopen the sheet with updated details, including when using native back. Returning from the barcode scanner must stay in the editor.
- Cancel deletion, then confirm it; cancellation keeps the sheet open, while confirmation closes it and refreshes the overview.
- Tap to zoom a barcode (including on a narrow screen): three distinct sizes cycle back to the initial size after three taps, without changing the card panel width. Text codes still copy on tap.
- At three-quarter height, scroll to the delete action and check that the bottom safe area matches the sheet background, in light and dark mode.
- Close or edit the card: brightness and screen sleep return to normal.
- Open a private card, reject authentication, and dismiss during authentication: no card data is exposed.

Card details use Plugin.Maui.BottomSheet 10.0.7 (MIT). Its navigation service presents the sheet
above the existing Shell overview; editing remains a Shell page and returning to the overview
reopens the card. On iOS, reopening waits for the native navigation transition to finish rather
than presenting during `OnAppearing`. Favorite status is preserved when saving edits.
The overview refreshes when the sheet is dismissed. Version 11.0.0 is not used
because its published dependency graph references missing native packages and the unrelated
Xamarin.Forms `Plugin.BottomSheet` package.
The sheet uses a themed `Background` brush; a compatibility bridge forwards its color to the
10.0.7 plugin's legacy native-background mapping so the bottom safe area is painted too.

---

## License

This project is licensed under the [MIT License](LICENSE).
