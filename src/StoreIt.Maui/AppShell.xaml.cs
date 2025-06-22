using StoreIt.Maui.Views;
using StoreIt.Navigation;

namespace StoreIt.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(Pages.AddCardPage, typeof(AddCardPage));
        Routing.RegisterRoute(Pages.ViewCardPage, typeof(ViewCardPage));
        Routing.RegisterRoute(Pages.ScanBarCodePage, typeof(ScanBarcodePage));
        Routing.RegisterRoute(Pages.AddBarCodePage, typeof(ManualBarcodePage));
    }
}
