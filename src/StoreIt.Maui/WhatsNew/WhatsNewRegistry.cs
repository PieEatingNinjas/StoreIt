using StoreIt.WhatsNew.Pages;

namespace StoreIt.WhatsNew;

public static class WhatsNewRegistry
{
    public static List<WhatsNewEntry> Items =>
    [
        new() { Id = 2, Version = "1.1.0", PageType = typeof(WhatsNew110Page) },
        new() { Id = 3, Version = "1.2.0", PageType = typeof(WhatsNew120Page) },
    ];
}

public class WhatsNewEntry
{
    public int Id { get; init; }
    public string? Version { get; init; } = string.Empty;
    public Type PageType { get; init; } = null!;
}