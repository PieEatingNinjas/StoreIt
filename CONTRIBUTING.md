# Contributing to StoreIt

Thank you for considering contributing to StoreIt! 🎉

## How to Contribute

### Reporting Bugs

Before opening a bug report, please check the [existing issues](../../issues) to avoid duplicates.

When reporting a bug, include:
- A clear title and description
- Steps to reproduce the issue
- Expected vs. actual behavior
- Device model and OS version
- App version (visible in **Settings → About**)

### Suggesting Features

Open an [issue](../../issues/new) with the label `enhancement` and describe:
- The problem your feature would solve
- A proposed solution or behavior
- Any alternatives you considered

### Submitting a Pull Request

1. **Fork** the repository and create a branch from `main`:
   ```bash
   git checkout -b feature/my-feature
   ```

2. **Make your changes** — keep commits focused and atomic.

3. **Build and test** your changes before submitting:
   ```bash
   dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net9.0-android
   dotnet build src/StoreIt.Maui/StoreIt.Maui.csproj -f net9.0-ios
   ```

4. **Open a pull request** against `main`. Include:
   - A description of what changed and why
   - Screenshots or recordings for UI changes
   - A reference to a related issue if applicable

## Code Style

- Follow the existing code patterns and naming conventions in the project.
- Use the [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) source generators for view models.
- Keep platform-specific code inside the `Platforms/` folder.
- Prefer constructor injection via the DI container registered in `MauiProgram.cs`.

## Getting Started

See the [README](README.md) for setup instructions.

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
