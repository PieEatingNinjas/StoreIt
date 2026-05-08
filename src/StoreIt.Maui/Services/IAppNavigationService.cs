namespace StoreIt.Services;

public interface IAppNavigationService : INavigationService
{
    Task NavigateToScanBarCodePage();
    Task NavigateToAddBarCodePage(string? barcode = null, string? barcodeFormat = null);
    Task NavigateToAddCardPage();
    Task NavigateToViewCardPage(int id);
    Task SwitchToScanBarCodePage();
    Task SwitchToManualBarCodePage();
    Task NavigateToEditCardPage(int id);
    Task GoToRoot();
    Task Show(Type pageType);
}