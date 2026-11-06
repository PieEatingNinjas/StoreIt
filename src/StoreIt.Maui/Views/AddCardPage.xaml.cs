using Microsoft.Maui.Layouts;
using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class AddCardPage : ContentPage
{
    // Minimum width per swatch (incl. breathing room) before falling back to two rows.
    private const double MinimumSwatchCellWidth = 44;

    private int _swatchColumns;

    public AddCardViewModel ViewModel { get; }

    public AddCardPage(AddCardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
    }

    private async void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        try
        {
            await ViewModel.SetIsPrivateCardCommand(e.Value);
        }
        catch
        {
            
        }
    }

    // One row when every swatch fits, otherwise evenly split rows (e.g. 2 x 4) so the grid stays balanced.
    private void ColorSwatches_SizeChanged(object? sender, EventArgs e)
    {
        var count = ColorSwatches.Children.Count;
        if (count == 0 || ColorSwatches.Width <= 0)
        {
            return;
        }

        var rows = 1;
        while (ColorSwatches.Width / Math.Ceiling((double)count / rows) < MinimumSwatchCellWidth && rows < count)
        {
            rows++;
        }

        var columns = (int)Math.Ceiling((double)count / rows);
        if (columns == _swatchColumns)
        {
            return;
        }

        _swatchColumns = columns;
        foreach (var child in ColorSwatches.Children)
        {
            ApplySwatchBasis(child);
        }
    }

    private void ColorSwatches_ChildAdded(object? sender, ElementEventArgs e)
    {
        _swatchColumns = 0;
        ColorSwatches_SizeChanged(sender, EventArgs.Empty);
    }

    private void ApplySwatchBasis(IView child)
    {
        if (child is BindableObject bindable && _swatchColumns > 0)
        {
            FlexLayout.SetBasis(bindable, new FlexBasis(1f / _swatchColumns, true));
        }
    }
}