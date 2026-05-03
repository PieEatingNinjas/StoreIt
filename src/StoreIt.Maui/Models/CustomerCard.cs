using SQLite;

namespace StoreIt.Maui.Models;

[Table("CustomerCards")]
public class CustomerCard
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public string? BarcodeData { get; set; }

    public string? BarcodeFormat { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime LastUsed { get; set; }

    public string? Color { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsPrivate { get; set; }

    public string? CustomCode { get; set; }

    public bool HasBarcode => !string.IsNullOrEmpty(BarcodeData);

    public bool HasCustomCode => !string.IsNullOrEmpty(CustomCode);
}