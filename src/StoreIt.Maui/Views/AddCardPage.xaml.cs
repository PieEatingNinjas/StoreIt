using StoreIt.Maui.ViewModels;

namespace StoreIt.Maui.Views;

public partial class AddCardPage : ContentPage
{
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
}