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

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        ViewModel.SetIsPrivateCardCommand(e.Value);
    }
}
