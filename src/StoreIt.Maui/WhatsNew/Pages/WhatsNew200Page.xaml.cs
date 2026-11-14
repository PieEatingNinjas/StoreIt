namespace StoreIt.WhatsNew.Pages;

public partial class WhatsNew200Page
{
    public WhatsNew200Page(WhatsNewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
