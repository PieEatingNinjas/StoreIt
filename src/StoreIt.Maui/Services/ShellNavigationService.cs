using StoreIt.Navigation;

namespace StoreIt.Services;

public class ShellNavigationService : IAppNavigationService
{
    public Task GoBack()
        => Shell.Current.GoToAsync("..");

    public Task NavigateToScanBarCodePage()
        => NavigateTo(Pages.ScanBarCodePage);

    public Task NavigateToAddBarCodePage(string? barcode = null, string? barcodeFormat = null)
    {
        var navigationParams = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(barcode))
        {
            navigationParams.Add(NavigationParams.BarcodeData, barcode);
        }

        if (!string.IsNullOrEmpty(barcodeFormat))
        {
            navigationParams.Add(NavigationParams.BarcodeFormat, barcodeFormat);
        }

        return NavigateTo(Pages.AddBarCodePage, navigationParams);
    }


    public Task NavigateTo(string route, Dictionary<string, object>? parameters = null)
    {
        if (parameters is not null)
        {
            return Shell.Current.GoToAsync(route, parameters);
        }
        else
        {
            return Shell.Current.GoToAsync(route);
        }
    }

    public Task NavigateToAddCardPage()
        => NavigateTo(Pages.AddCardPage);

    public Task NavigateToViewCardPage(int id)
        => NavigateTo(Pages.ViewCardPage, new Dictionary<string, object>
        {
            { NavigationParams.CardId, id }
        });

    public Task SwitchToScanBarCodePage()
        => NavigateTo($"../{Pages.ScanBarCodePage}");

    public Task GoBack(Dictionary<string, object> parameters)
        => Shell.Current.GoToAsync("..", parameters);

    public Task SwitchToManualBarCodePage()
        => NavigateTo($"../{Pages.AddBarCodePage}");

    public Task NavigateToEditCardPage(int id)
        => NavigateTo(Pages.AddCardPage, new Dictionary<string, object>
        {
            { NavigationParams.CardId, id }
        });

    public Task GoToRoot()
        => NavigateTo($"/{Pages.MainPage}");

    public async Task Show(Type pageType)
    {
        Routing.RegisterRoute("temp", pageType);
        await Shell.Current.GoToAsync("temp");
        Routing.UnRegisterRoute("temp");
    }
}