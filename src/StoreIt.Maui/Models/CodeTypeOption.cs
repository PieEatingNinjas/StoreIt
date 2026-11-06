using CommunityToolkit.Mvvm.ComponentModel;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Models;

public partial class CodeTypeOption : ObservableObject
{
    public required string Format { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public bool IsBarcode { get; init; }
    public bool IsSquare { get; init; }
    public BarcodeFormat BarcodeFormat { get; init; } = BarcodeFormat.QrCode;
    public string SampleValue { get; init; } = string.Empty;

    public double PreviewWidth => IsSquare ? 90 : 220;

    public bool IsText => !IsBarcode;

    public string StatusText => IsSample ? "Voorbeeld" : IsValid ? string.Empty : "Past niet bij je code";

    [ObservableProperty]
    private string previewValue = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool isSample;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool isValid = true;

    [ObservableProperty]
    private bool isSelected;
}
