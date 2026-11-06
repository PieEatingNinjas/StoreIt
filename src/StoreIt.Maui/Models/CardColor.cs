using CommunityToolkit.Mvvm.ComponentModel;

namespace StoreIt.Maui.Models;

public partial class CardColor : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    // Brush-typed properties (e.g. Border.Stroke) cannot convert a hex string in a binding.
    public Brush Brush => new SolidColorBrush(Color.FromArgb(Value));

    [ObservableProperty]
    private bool isSelected;
}
