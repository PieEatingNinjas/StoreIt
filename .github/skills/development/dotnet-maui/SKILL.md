---
name: dotnet-maui
description: .NET MAUI UI knowledge — modern controls, compiled bindings, Shell navigation, handlers, and the obsolete patterns to never use. Load when building or reviewing MAUI XAML/UI code.
---

# .NET MAUI

> **Attribution:** adapted from the *.NET MAUI Coding Expert* agent in
> [github/awesome-copilot](https://awesome-copilot.github.com/agent/dotnet-maui/) (MIT). Reshaped

This skill is **UI/controls knowledge**, not a role. It tells the DeveloperAgent how to build MAUI
UI correctly and gives the ArchitectureAgent a concrete MAUI review lens. Process, MVVM structure,
async, DI and scope discipline come from the agents + `copilot-instructions.md` — this skill layers
the MAUI-specific "what to use / what to never use" on top.

## Critical rules (never violate)

These are hard "no"s — flag any occurrence as a problem, don't just prefer the alternative.

- **No `ListView`** — obsolete. Use `CollectionView`.
- **No `TableView`** — obsolete. Use `Grid` / `VerticalStackLayout`.
- **No `AndExpand` layout options** (`*AndExpand`) — obsolete.
- **No `BackgroundColor`** — use the `Background` property.
- **No `ScrollView` or `CollectionView` inside a `StackLayout`** — breaks scrolling/virtualization.
- **No `.svg` in `Source` references** — reference the **PNG** (SVG is a build-time source only).
- **No mixing `Shell` with `NavigationPage` / `TabbedPage` / `FlyoutPage`** — pick one model.
- **No renderers** — use **handlers**.

## Control selection (quick reference)

| Need | Use | Not |
|------|-----|-----|
| List > 20 items (virtualized) | `CollectionView` | `ListView` |
| Small list ≤ 20 items | `BindableLayout` | `CollectionView` overkill |
| Gallery / onboarding | `CarouselView` + `IndicatorView` | — |
| Container with border/shadow | `Border` (`StrokeShape="RoundRectangle 10"`) | `Frame` (legacy; shadows only) |
| Complex layout | `Grid` (`RowDefinitions`/`ColumnDefinitions`) | deeply nested stacks |
| Vertical/horizontal stack | `VerticalStackLayout` / `HorizontalStackLayout` | `StackLayout Orientation=...` |
| Busy (indeterminate) | `ActivityIndicator` | — |
| Progress (0.0–1.0) | `ProgressBar` | — |
| Pull-to-refresh | `RefreshView` | — |
| Swipe actions | `SwipeView` | — |
| Custom drawing | `GraphicsView` (`ICanvas`) | — |

Multi-line text: `Editor` (`AutoSize="TextChanges"`). Single-line: `Entry`.

## Compiled bindings (performance — do this by default)

Always set `x:DataType` on pages/templates — compiled bindings are 8–20× faster and type-safe.

```xml
<ContentPage x:DataType="vm:MainViewModel">
    <Label Text="{Binding Name}" />
</ContentPage>
```

In C#, prefer expression-based bindings over string paths:

```csharp
// DO — type-safe, compiled
label.SetBinding(Label.TextProperty, static (PersonViewModel vm) => vm.FullName?.FirstName);
// DON'T — string paths: runtime errors, no IntelliSense
label.SetBinding(Label.TextProperty, "FullName.FirstName");
```

**Binding modes:** `OneTime` (never changes) · `OneWay` (default, read-only) · `TwoWay` (only for
editable). Don't bind static values — set them directly.

## Layout do's

```xml
<!-- Grid for structure -->
<Grid RowDefinitions="Auto,*" ColumnDefinitions="*,*">
<!-- Border instead of Frame -->
<Border Stroke="Black" StrokeThickness="1" StrokeShape="RoundRectangle 10">
<!-- Specific stack layouts -->
<VerticalStackLayout>
```

Flatten deep hierarchies — nested layouts cost measure/arrange passes.

## Shell navigation (recommended)

```csharp
Routing.RegisterRoute("details", typeof(DetailPage));
await Shell.Current.GoToAsync("details?id=123");
```

- Set `MainPage` **once** at startup; don't swap it frequently.
- Don't nest tabs.

## Handlers, not renderers

Customize platform visuals via handler mappers in `MauiProgram.cs`:

```csharp
Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("Custom", (handler, view) =>
{
#if ANDROID
    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.HotPink);
#elif IOS
    handler.PlatformView.BackgroundColor = UIKit.UIColor.SystemPink;
#endif
});
```

## Platform-specific code

```csharp
#if ANDROID
#elif IOS
#elif WINDOWS
#elif MACCATALYST
#endif
```

UI updates from a background thread: prefer injecting `IDispatcher` (or `BindableObject.Dispatcher`);
`MainThread.BeginInvokeOnMainThread(...)` is the fallback.

## Security

- Store tokens/secrets in `SecureStorage`, never in code, config, or `Preferences`:

```csharp
await SecureStorage.SetAsync("oauth_token", token);
string? token = await SecureStorage.GetAsync("oauth_token");
```

- Validate all external input; use **HTTPS** for every remote call.

## Resources

- `Resources/Images/` (PNG/JPG; SVG is source-only → referenced as PNG), `Resources/Fonts/`,
  `Resources/Raw/`.
- Reference as PNG: `<Image Source="logo.png" />` — never `.svg`.
- Size images appropriately to avoid memory bloat.

## Common pitfalls (review lens)

1. Mixing `Shell` with `NavigationPage`/`TabbedPage`/`FlyoutPage`.
2. Swapping `MainPage` frequently; nesting tabs.
3. Gesture recognizers on both parent and child (set `InputTransparent="true"` on the parent).
4. Renderers instead of handlers.
5. **Memory leaks from unsubscribed events** — unsubscribe in `OnDisappearing`/`Dispose`.
6. Deeply nested layouts — flatten.
7. Testing only on emulators — verify on real devices.
8. Assuming every Xamarin.Forms API exists in MAUI — check before use.

## Verify before asserting

MAUI/handler/Shell APIs move fast. When unsure of a signature or availability, check the
**Microsoft Learn MCP** (`microsoft_docs_search` / `microsoft_code_sample_search`) rather than
guessing.
