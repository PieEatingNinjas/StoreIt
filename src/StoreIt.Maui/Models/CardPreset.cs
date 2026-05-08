namespace StoreIt.Maui.Models;

public class CardPreset
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? LogoEmoji { get; set; }
    public string? Description { get; set; }
    public bool RequiresBarcode { get; set; } = true;
    public bool IsCustom { get; set; } = false;
}