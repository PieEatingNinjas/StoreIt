namespace StoreIt.Services;

public interface IAppNavigationService : INavigationService
{
    Task NavigateToScanBarCodePage();
    Task NavigateToAddCardPage();
    Task NavigateToViewCardPage(int id);
    Task NavigateToEditCardPage(int id);
    Task GoToRoot();
    Task Show(Type pageType);
}