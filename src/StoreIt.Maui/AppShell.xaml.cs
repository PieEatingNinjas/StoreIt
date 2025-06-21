using StoreIt.Maui.Views;

namespace StoreIt.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
          // Register routes for navigation
        Routing.RegisterRoute("addcard", typeof(AddCardPage));
        Routing.RegisterRoute("viewcard", typeof(ViewCardPage));
        Routing.RegisterRoute("scanbarcode", typeof(ScanBarcodePage));
        Routing.RegisterRoute("manualbarcode", typeof(ManualBarcodePage));
    }
}
