using StoreIt.WhatsNew.Pages;

namespace StoreIt.WhatsNew;

public static class WhatsNewRegistry
{
    public static List<WhatsNewEntry> Items => new()
    {
        new WhatsNewEntry { Id = 2, Version = "1.1.0", PageType = typeof(WhatsNew110Page) },
        // new WhatsNewEntry { Id = 2, Version = "1.1.0", PageType = typeof(WhatsNew110Page) },
    };
}

public class WhatsNewEntry
{
    public int Id { get; set; }
    public string? Version { get; set; } = string.Empty;
    public Type PageType { get; set; } = null!;
}