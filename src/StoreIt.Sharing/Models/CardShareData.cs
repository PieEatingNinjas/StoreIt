using System.Text.Json.Serialization;

namespace StoreIt.Sharing.Models;

public sealed record CardShareData(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("barcodeData")] string? BarcodeData,
    [property: JsonPropertyName("barcodeFormat")] string? BarcodeFormat,
    [property: JsonPropertyName("customCode")] string? CustomCode,
    [property: JsonPropertyName("color")] string? Color,
    [property: JsonPropertyName("isFavorite")] bool IsFavorite,
    [property: JsonPropertyName("isPrivate")] bool IsPrivate);
